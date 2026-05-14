using DevArena.Data;
using DevArena.Repos;
using Microsoft.AspNetCore.Mvc;

namespace DevArena.Web.Controllers
{
    public class Contests(ContestsRepo contestsRepo) : Controller
    {

        public IActionResult Index()
        {
            var result =  contestsRepo.GetAll();
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            return View(result.Data);
        }
    }
}
