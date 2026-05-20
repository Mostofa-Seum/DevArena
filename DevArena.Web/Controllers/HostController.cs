using DevArena.Models;
using DevArena.Repos;
using DevArena.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DevArena.Web.Controllers
{
    public class HostController(HostRepo hostRepo, CurrentUserHelper currentUserHelper) : Controller
    {
        public IActionResult GetParticipantsInfo()
        {
            int hostId = currentUserHelper.UserId;
            var result = hostRepo.GetParticipantRegistrations(hostId);

            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(new List<RegistrationModel>());
            }
            return View(result.Data);
        }

        [HttpPost]
        public IActionResult Promote(int participantId, int contestId)
        {
            int hostId = currentUserHelper.UserId;
            var result = hostRepo.PromoteToJudge(participantId, contestId, hostId);

            if (result.HasError)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Participant successfully promoted to Judge!";
            }

            return RedirectToAction(nameof(GetParticipantsInfo));
        }

        [HttpPost]
        public IActionResult Remove(int participantId, int contestId)
        {
            var result = hostRepo.DeactivateParticipant(participantId, contestId);

            if (result.HasError)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Participant successfully removed from the contest.";
            }

            return RedirectToAction(nameof(GetParticipantsInfo));
        }

        [HttpPost]
        public IActionResult Demote(int participantId, int contestId)
        {
            var result = hostRepo.DemoteFromJudge(participantId, contestId);

            if (result.HasError)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Judge successfully demoted back to Participant!";
            }

            return RedirectToAction(nameof(GetParticipantsInfo));
        }
    }
}