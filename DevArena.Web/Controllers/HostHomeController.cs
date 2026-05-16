using System.Diagnostics;
using DevArena.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevArena.Web.Controllers
{

    public class HostHomeController : Controller
    {
        private readonly ILogger<HostHomeController> _logger;

        public HostHomeController(ILogger<HostHomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
