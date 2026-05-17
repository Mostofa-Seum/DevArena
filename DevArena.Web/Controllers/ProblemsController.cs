using DevArena.Data;
using DevArena.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevArena.Web.Controllers
{
    public class ProblemsController(ProblemsRepo problemsRepo, DevArenaDbContext context) : Controller
    {
        public async Task<IActionResult> ContestProblems(int contestId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var participant = await context.Participants.FirstOrDefaultAsync(p => p.email == userEmail);

            if (participant == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var contest = await context.Contests.FindAsync(contestId);

            if (contest == null || !contest.is_active)
            {
                TempData["Error"] = "This contest is currently inactive. You cannot view its problems.";
                return RedirectToAction("Index", "Contests");
            }

            var isJoined = await context.ContestRegistrations
                .AnyAsync(cr => cr.contest_id == contestId
                             && cr.participant_id == participant.id
                             && cr.is_active == true);

            if (!isJoined)
            {
                // FIX: Changed "ErrorMessage" to "Error"
                TempData["Error"] = "You must join the contest before you can view the problems.";
                return RedirectToAction("IsActive", "Contests");
            }

            var problems = await problemsRepo.GetProblemsByContestIdAsync(contestId);

            ViewBag.ContestId = contestId;

            return View(problems);
        }

        public IActionResult Solve(int id)
        {
            // Placeholder for your solving logic
            return View();
        }
    }
}