using DevArena.Data;
using DevArena.Repos;
using Microsoft.AspNetCore.Mvc;

namespace DevArena.Web.Controllers
{
    public class Contests(ContestsRepo contestsRepo) : Controller
    {

        public IActionResult IsActive()
        {
            var result =  contestsRepo.GetActive();
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            return View(result.Data);
        }
        public IActionResult IsInActive()
        {
            var result =  contestsRepo.GetInActive();
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            return View(result.Data);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
