using DevArena.Data;
using DevArena.Entities;
using DevArena.Repos;
using DevArena.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DevArena.Web.Controllers
{
    public class ContestsController(ContestsRepo contestsRepo) : Controller
    {

        public IActionResult IsActive()
        {
            var result = contestsRepo.GetActive();
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            return View(result.Data);
        }
        public IActionResult IsInActive()
        {
            var result = contestsRepo.GetInActive();
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            return View(result.Data);
        }

        public IActionResult Index()
        {

            var result = contestsRepo.GetAllByCurrentHost();

            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(new List<Contests>());
            }
            return View(result.Data);
        }

        public IActionResult Details(int id)
        {
            if(id == -1)
            {
                return View (new Contests());
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = contestsRepo.Save(model);

            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
            }
            else
            {
                if(result.Data != null) TempData["Success"] = $"Contest with ID {result.Data.id} has been successfully saved.";
                return RedirectToAction("Index");
            }
            return View(result.Data);
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
