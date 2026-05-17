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
                TempData["Error"] = "You must join the contest before you can view the problems.";
                return RedirectToAction("IsActive", "Contests");
            }

            var problems = await problemsRepo.GetProblemsByContestIdAsync(contestId);

            ViewBag.ContestId = contestId;

            return View(problems);
        }

        [HttpGet]
        public async Task<IActionResult> Solve(int id)
        {
            var problem = await context.Problems.FindAsync(id);

            if (problem == null)
            {
                TempData["Error"] = "The requested problem could not be found.";
                return RedirectToAction("Index", "Contests");
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var participant = await context.Participants.FirstOrDefaultAsync(p => p.email == userEmail);

            if (participant == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            bool hasSubmitted = await context.Submissions
                .AnyAsync(s => s.participant_id == participant.id && s.problem_id == id);

            ViewBag.HasSubmitted = hasSubmitted;

            return View(problem);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCode(int problem_id, int contest_id, string language, string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                TempData["Error"] = "You cannot submit empty code.";
                return RedirectToAction("Solve", new { id = problem_id });
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var participant = await context.Participants.FirstOrDefaultAsync(p => p.email == userEmail);

            if (participant == null)
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            // SECURITY CHECK: Verify they haven't submitted already (in case they refresh the form submission)
            bool hasSubmitted = await context.Submissions
                .AnyAsync(s => s.participant_id == participant.id && s.problem_id == problem_id);

            if (hasSubmitted)
            {
                TempData["Error"] = "You have already submitted a solution for this problem. Multiple submissions are not allowed.";
                return RedirectToAction("ContestProblems", new { contestId = contest_id });
            }

            var submission = new DevArena.Entities.Submission
            {
                participant_id = participant.id,
                contest_id = contest_id,
                problem_id = problem_id,
                code_text = sourceCode,
                status = "Pending",
                submitted_at = DateTime.UtcNow
            };

            try
            {
                context.Submissions.Add(submission);
                await context.SaveChangesAsync();

                TempData["Success"] = "Code submitted successfully! Pending judgment...";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "An error occurred while saving your submission. Please try again.";
                return RedirectToAction("Solve", new { id = problem_id });
            }

            return RedirectToAction("ContestProblems", new { contestId = contest_id });
        }
    }
}