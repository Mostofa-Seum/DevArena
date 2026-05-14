using Microsoft.AspNetCore.Mvc;

namespace DevArena.Web.Controllers
{
    public class Contests : Controller
    {
        public IActionResult Index()
        {
            return Content("Hello, World!");
        }
    }
}
