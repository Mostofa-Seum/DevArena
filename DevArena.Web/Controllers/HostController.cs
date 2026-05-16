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
    }
}