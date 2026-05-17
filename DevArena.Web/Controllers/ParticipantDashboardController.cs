using DevArena.Data;
using DevArena.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevArena.Web.Controllers
{
    public class ParticipantDashboardController : Controller
    {
        private readonly DevArenaDbContext _context;

        public ParticipantDashboardController(DevArenaDbContext context)
        {
            _context = context;
        }

        // 1. Show the Dashboard
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.email == userEmail);

            if (participant == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(participant);
        }

        // 2. Update Profile Information (Name & Email)
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string name, string email)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.email == userEmail);

            if (participant == null) return RedirectToAction("Login", "Auth");

            // Check if they are trying to change to an email that is already taken
            if (email != userEmail)
            {
                var emailExists = await _context.Participants.AnyAsync(p => p.email == email);
                if (emailExists)
                {
                    TempData["Error"] = "This email is already in use by another account.";
                    return RedirectToAction("Index");
                }
            }

            participant.name = name;
            participant.email = email;

            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();

            // If they changed their email, their current login session (cookie) is now invalid
            // because it stores the old email. We must log them out to force a fresh login.
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
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.email == userEmail);

            if (participant == null) return RedirectToAction("Login", "Auth");

            // Verify Old Password
            if (participant.password != oldPassword)
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
            participant.password = newPassword;
            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password changed successfully!";
            return RedirectToAction("Index");
        }
    }
}