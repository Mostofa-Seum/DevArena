using DevArena.Data;
using Microsoft.AspNetCore.Mvc;

namespace DevArena.Web.Controllers
{
    public class Contests(DevArenaDbContext context) : Controller
    {

        public IActionResult Index()
        {
            return Content(context.Contests.ToList().Count.ToString());
        }
    }
}
