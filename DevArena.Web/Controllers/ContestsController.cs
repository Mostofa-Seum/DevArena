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

        public IActionResult Delete(int dataId)
        {
            var result = contestsRepo.Delete(dataId);
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            return RedirectToAction("Index");
        }
    }
}
