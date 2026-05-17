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
            // Fetch the specific problem from the database
            var problem = await context.Problems.FindAsync(id);

            if (problem == null)
            {
                TempData["Error"] = "The requested problem could not be found.";
                return RedirectToAction("Index", "Contests");
            }

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

            // 1. Identify the logged-in participant
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var participant = await context.Participants.FirstOrDefaultAsync(p => p.email == userEmail);

            if (participant == null)
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            // 2. Create the new Submission record
            var submission = new DevArena.Entities.Submission
            {
                participant_id = participant.id,
                contest_id = contest_id,
                problem_id = problem_id,
                code_text = sourceCode,
                status = "Pending", // Initial status before the Judge reviews it
                submitted_at = DateTime.UtcNow

                // Note: 'language' is passed from the view, but your Submission.cs 
                // doesn't have a language column to save it into right now.
            };

            // 3. Save it to the database
            try
            {
                context.Submissions.Add(submission);
                await context.SaveChangesAsync();

                TempData["Success"] = "Code submitted successfully! Pending judgment...";
            }
            catch (System.Exception)
            {
                // Catch any database errors
                TempData["Error"] = "An error occurred while saving your submission. Please try again.";
                // Optionally log the exception 'ex' here
                return RedirectToAction("Solve", new { id = problem_id });
            }

            // 4. Redirect the user back to the problems list
            return RedirectToAction("ContestProblems", new { contestId = contest_id });
        }


    }
}