using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DevArena.Data;
using DevArena.Models;

public class AuthController(DevArenaDbContext context) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string role = null;
        string userId = null;
        string userName = null;

        // Check if the user is an Admin
        var admin = context.Admins.FirstOrDefault(a => a.email == model.email && a.password == model.password);
        if (admin != null)
        {
            // Block Admins from logging in as Judges
            if (model.IsJudgeLogin)
            {
                ViewBag.Error = "Admins cannot log in as Judges.";
                return View(model);
            }

            role = "Admin";
            userId = admin.id.ToString();
            userName = admin.name;
        }
        else
        {
            // Check if the user is a Host
            var host = context.Hosts.FirstOrDefault(h => h.email == model.email && h.password == model.password);
            if (host != null)
            {
                // Block Hosts from logging in as Judges
                if (model.IsJudgeLogin)
                {
                    ViewBag.Error = "Hosts cannot log in as Judges.";
                    return View(model);
                }

                role = "Host";
                userId = host.id.ToString();
                userName = host.name;
            }
            else
            {
                // Check if the user is a Participant
                var participant = context.Participants.FirstOrDefault(p => p.email == model.email && p.password == model.password);
                if (participant != null)
                {
                    userId = participant.id.ToString();
                    userName = participant.name;

                    // CHECKBOX LOGIC: Verify if they want to log in as a Judge
                    if (model.IsJudgeLogin)
                    {
                        int pId = participant.id;
                        var judgeRecord = context.Judges.FirstOrDefault(j => j.participant_id == pId);

                        if (judgeRecord != null)
                        {
                            // Check if the judge status is active
                            if (judgeRecord.Is_active)
                            {
                                role = "Judge"; // Assign Judge role
                            }
                            else
                            {
                                // Trigger error if they are in the judge table but not active
                                ViewBag.Error = "Your judge account is currently inactive.";
                                return View(model);
                            }
                        }
                        else
                        {
                            // Trigger error if they checked the box but aren't in the Judges table
                            ViewBag.Error = "You are not a judge.";
                            return View(model);
                        }
                    }
                    else
                    {
                        role = "Participant"; // Default if box is unchecked
                    }
                }
            }
        }

        // If no match was found in any table, authentication fails
        if (role == null)
        {
            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }

        // Create the user's identity claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, model.email),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "DAAuth");

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("DAAuth", principal);

        // Redirect based on role
        if (role == "Admin") return RedirectToAction("Index", "AdminDashboard");
        if (role == "Host") return RedirectToAction("Index", "HostHome");
        if (role == "Participant") return RedirectToAction("Index", "ParticipantHome");
        // Redirect for Judge 
        if (role == "Judge") return RedirectToAction("Index", "Home");

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("DAAuth");
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult Signup()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignupModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if the email already exists in either table to prevent duplicates
        bool emailInParticipants = context.Participants.Any(p => p.email == model.Email);
        bool emailInHosts = context.Hosts.Any(h => h.email == model.Email);

        if (emailInParticipants || emailInHosts)
        {
            ViewBag.Error = "This email is already registered.";
            return View(model);
        }

        if (model.Role == "Participant")
        {
            var participant = new DevArena.Entities.Participants
            {
                name = model.Name,
                email = model.Email,
                password = model.Password,
                Is_active = true,
                created_at = System.DateTime.UtcNow
            };
            context.Participants.Add(participant);
        }
        else if (model.Role == "Host")
        {
            var host = new DevArena.Entities.Host
            {
                name = model.Name,
                email = model.Email,
                password = model.Password,
                Is_active = true,
                created_at = System.DateTime.Now
            };
            context.Hosts.Add(host);
        }
        else
        {
            ViewBag.Error = "Invalid role selected.";
            return View(model);
        }

        await context.SaveChangesAsync();

        TempData["Success"] = "Account created successfully! You can now log in.";
        return RedirectToAction("Login");
    }
}