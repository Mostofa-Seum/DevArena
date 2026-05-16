using Microsoft.AspNetCore.Mvc;

namespace DevArena.Web.Controllers
{
    public class ParticipantHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
