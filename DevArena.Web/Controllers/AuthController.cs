using DevArena.Models;
using DevArena.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace DevArena.Web.Controllers
{
    public class AuthController(HostRepo repo) : Controller
    {
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(ModelState.IsValid == false)
            {
                return View(model);
            }
            var result = repo.Authenticate(model.email, model.password);
            if (result.HasError || result.Data == null)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }
            var claims = new List<Claim>()
{
                new Claim(ClaimTypes.Name, result.Data.name),
                new Claim(ClaimTypes.Email, result.Data.email),
                new Claim("UserID", result.Data.id.ToString()),
                new Claim("UserType", "Host")

};
            var identity = new ClaimsIdentity(claims, "DAAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("DAAuth", principal);

                return Content("Hello Host");// need to redirect to host dashboard
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("DAAuth");
            return RedirectToAction("Login");
        }

        public IActionResult Denied()
        {
            return View();
        }
    }
}
