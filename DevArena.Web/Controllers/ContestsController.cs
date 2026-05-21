using DevArena.Data;
using DevArena.Entities;
using DevArena.Repos;
using DevArena.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;

namespace DevArena.Web.Controllers
{
    public class ContestsController(ContestsRepo contestsRepo, CurrentUserHelper currentUserHelper, ContestRegistrationRepo regRepo) : Controller
    {
        private readonly CurrentUserHelper _currentUserHelper = currentUserHelper;

        public IActionResult IsActive()
        {
            var result = contestsRepo.GetActive();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new List<Contests>());
            }

            ViewBag.RegisteredContests = new List<int>();

            if (User.IsInRole("Participant"))
            {
                var registrationResult = regRepo.GetRegisteredContestIds();

                if (!registrationResult.HasError && registrationResult.Data != null)
                {
                    ViewBag.RegisteredContests = registrationResult.Data;
                }
            }

            return View(result.Data);
        }

        public IActionResult IsInActive()
        {
            var result = contestsRepo.GetInActive();
            if (result.HasError)
            {

                ViewBag.Error = result.Message;
                return View();
            }
            return View(result.Data);
        }

        public IActionResult Index()
        {
            var result = contestsRepo.GetAllByCurrentHost();

            if (result.HasError)
            {

                ViewBag.Error = result.Message;
                return View(new List<Contests>());
            }
            return View(result.Data);
        }

        public IActionResult Details(int id)
        {
            if (id == -1)
            {
                var newContest = new Contests
                {
                    start_time = DateTime.UtcNow,
                    end_time = DateTime.UtcNow.AddHours(2),
                    is_active = true
                };
                return View(newContest);
            }

            var result = contestsRepo.GetById(id);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }

        [HttpPost]
        public IActionResult Details(Contests model)
        {
            model.host_id = _currentUserHelper.UserId;
            if (model.start_time == default(DateTime) || model.start_time == DateTime.MinValue)
            {
                model.start_time = DateTime.UtcNow;
            }
            if (model.end_time == default(DateTime) || model.end_time == DateTime.MinValue)
            {
                model.end_time = model.start_time.AddHours(2);
            }

            ModelState.Remove("Hosts");
            ModelState.Remove("host_id");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = contestsRepo.Save(model);

            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            if (result.Data != null)
            {
                TempData["Success"] = $"Contest with ID {result.Data.id} has been successfully saved.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var result = contestsRepo.Delete(id);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {

                TempData["Success"] = $"Contest with ID {id} has been successfully deleted.";
            }
            return RedirectToAction("Index");
        }
    }
}