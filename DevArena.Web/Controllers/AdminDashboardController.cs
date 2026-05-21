using DevArena.Entities;
using DevArena.Repos;
using DevArena.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DevArena.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly AdminRepo _adminRepo;

        public AdminDashboardController(AdminRepo adminRepo)
        {
            _adminRepo = adminRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var adminResult = await _adminRepo.GetAdminByEmailAsync(userEmail);

            if (adminResult.HasError || adminResult.Data == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var recentContests = await _adminRepo.GetRecentContestsAsync(5);

            var model = new AdminDashboardViewModel
            {
                TotalAdmins = (await _adminRepo.GetCountAsync<Admin>()).Data,
                TotalHosts = (await _adminRepo.GetCountAsync<DevArena.Entities.Host>()).Data,
                TotalJudges = (await _adminRepo.GetCountAsync<Judge>()).Data,
                TotalParticipants = (await _adminRepo.GetCountAsync<Participants>()).Data,
                TotalContests = (await _adminRepo.GetCountAsync<Contests>()).Data,
                TotalProblems = (await _adminRepo.GetCountAsync<Problems>()).Data,
                TotalSubmissions = (await _adminRepo.GetCountAsync<Submission>()).Data,
                CurrentAdmin = adminResult.Data,
                RecentContests = recentContests.Data ?? new List<Contests>()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Participants()
        {
            var result = await _adminRepo.GetAllParticipantsAsync();
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return View(new List<Participants>());
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Hosts()
        {
            var result = await _adminRepo.GetAllHostsAsync();
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return View(new List<DevArena.Entities.Host>());
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> ContestsList()
        {
            var result = await _adminRepo.GetAllContestsAsync();
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return View(new List<Contests>());
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _adminRepo.GetAdminByEmailAsync(userEmail);
            
            if (result.HasError || result.Data == null) return NotFound();
            
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Admin model)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminRepo.UpdateAdminAsync(model);
                if (!result.HasError)
                {
                    TempData["Success"] = "Profile updated successfully.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleParticipantStatus(int id)
        {
            var result = await _adminRepo.ToggleParticipantStatusAsync(id);
            if (!result.HasError)
            {
                TempData["Success"] = $"Participant status updated to {(result.Data ? "Active" : "Inactive")}.";
            }
            return RedirectToAction(nameof(Participants));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            var result = await _adminRepo.DeleteParticipantAsync(id);
            if (!result.HasError)
            {
                TempData["Success"] = "Participant deleted successfully.";
            }
            return RedirectToAction(nameof(Participants));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleHostStatus(int id)
        {
            var result = await _adminRepo.ToggleHostStatusAsync(id);
            if (!result.HasError)
            {
                TempData["Success"] = $"Host status updated to {(result.Data ? "Active" : "Inactive")}.";
            }
            return RedirectToAction(nameof(Hosts));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHost(int id)
        {
            var result = await _adminRepo.DeleteHostAsync(id);
            if (!result.HasError)
            {
                TempData["Success"] = "Host deleted successfully.";
            }
            return RedirectToAction(nameof(Hosts));
        }

        [HttpPost]
        public async Task<IActionResult> CancelContest(int id)
        {
            var result = await _adminRepo.CancelContestAsync(id);
            if (!result.HasError)
            {
                TempData["Success"] = "Contest canceled successfully.";
            }
            return RedirectToAction(nameof(ContestsList));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContest(int id)
        {
            var result = await _adminRepo.DeleteContestAsync(id);
            if (!result.HasError)
            {
                TempData["Success"] = "Contest deleted successfully.";
            }
            return RedirectToAction(nameof(ContestsList));
        }
    }
}