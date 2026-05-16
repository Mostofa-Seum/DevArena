using DevArena.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevArena.Web.Controllers
{
    public class ContestRegistrationController(ContestRegistrationRepo regRepo) : Controller
    {
        public IActionResult JoinContest(int id)
        {
            var result = regRepo.Join(id);

            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = "Successfully joined the contest!";
            }

            // Redirect back to the IsActive page inside the Contests folder
            return RedirectToAction("IsActive", "Contests");
        }

        public IActionResult LeaveContest(int id)
        {
            var result = regRepo.Leave(id);

            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = "You have successfully left the contest.";
            }
            return RedirectToAction("IsActive", "Contests");
        }
    }
}