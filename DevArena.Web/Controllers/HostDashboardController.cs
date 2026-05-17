using DevArena.Data;
using DevArena.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevArena.Web.Controllers
{
    [Authorize(Roles = "Host")]
    public class HostDashboardController : Controller
    {
        private readonly DevArenaDbContext _context;

        public HostDashboardController(DevArenaDbContext context)
        {
            _context = context;
        }

        // 1. Show the Dashboard
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Note: Make sure your DbContext has a DbSet<Host> Hosts property
            var host = await _context.Hosts.FirstOrDefaultAsync(h => h.email == userEmail);

            if (host == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(host);
        }

        // 2. Update Profile Information (Name & Email)
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string name, string email)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var host = await _context.Hosts.FirstOrDefaultAsync(h => h.email == userEmail);

            if (host == null) return RedirectToAction("Login", "Auth");

            // Check if they are trying to change to an email that is already taken by another host
            if (email != userEmail)
            {
                var emailExists = await _context.Hosts.AnyAsync(h => h.email == email);
                if (emailExists)
                {
                    TempData["Error"] = "This email is already in use by another account.";
                    return RedirectToAction("Index");
                }
            }

            host.name = name;
            host.email = email;

            _context.Hosts.Update(host);
            await _context.SaveChangesAsync();

            // If the email changed, force a re-login so the claims cookie updates
            if (email != userEmail)
            {
                TempData["Success"] = "Profile updated! Since you changed your email, please log in again.";
                return RedirectToAction("Logout", "Auth");
            }

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }

        // 3. Change Password
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var host = await _context.Hosts.FirstOrDefaultAsync(h => h.email == userEmail);

            if (host == null) return RedirectToAction("Login", "Auth");

            // Verify Old Password
            if (host.password != oldPassword)
            {
                TempData["Error"] = "Incorrect old password.";
                return RedirectToAction("Index");
            }

            // Verify New Passwords Match
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return RedirectToAction("Index");
            }

            // Verify Password Isn't Empty
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["Error"] = "Password cannot be empty.";
                return RedirectToAction("Index");
            }

            // Save new password
            host.password = newPassword;
            _context.Hosts.Update(host);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password changed successfully!";
            return RedirectToAction("Index");
        }
    }
}