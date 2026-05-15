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

        // 1. Check if the user is an Admin
        var admin = context.Admins.FirstOrDefault(a => a.email == model.email && a.password == model.password);
        if (admin != null)
        {
            role = "Admin";
            userId = admin.id.ToString();
            userName = admin.name;
        }
        else
        {
            // 2. Check if the user is a Host
            var host = context.Hosts.FirstOrDefault(h => h.email == model.email && h.password == model.password);
            if (host != null)
            {
                role = "Host";
                userId = host.id.ToString();
                userName = host.name;
            }
            else
            {
                // 3. Check if the user is a Participant
                var participant = context.Participants.FirstOrDefault(p => p.email == model.email && p.password == model.password);
                if (participant != null)
                {
                    role = "Participant";
                    userId = participant.id.ToString();
                    userName = participant.name;
                }
            }
        }

        // 4. If no match was found in any table, authentication fails
        if (role == null)
        {
            ViewBag.ErrorMessage = "Invalid email or password.";
            return View(model);
        }

        // 5. Create the user's identity claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, model.email),
            new Claim(ClaimTypes.Role, role) // This is where the Role is assigned!
        };

        // *Optional Logic for Judges*
        // Since Judges are actually Participants linked to a contest, 
        // you can give them a secondary "Judge" role if they exist in the judges table.
        if (role == "Participant")
        {
            int pId = int.Parse(userId);
            bool isAlsoJudge = context.Judges.Any(j => j.participant_id == pId);
            if (isAlsoJudge)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Judge"));
            }
        }

        var identity = new ClaimsIdentity(claims, "DAAuth");

        var principal = new ClaimsPrincipal(identity);

        // 6. Issue the authentication cookie
        await HttpContext.SignInAsync("DAAuth", principal);

        // Redirect based on role
        if (role == "Admin") return RedirectToAction("Index", "AdminDashboard");
        if (role == "Host") return RedirectToAction("Index", "HostDashboard");
        if (role == "Participant") return RedirectToAction("Index", "ParticipantDashboard");


        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("DAAuth");
        return RedirectToAction("Login");
    }
}