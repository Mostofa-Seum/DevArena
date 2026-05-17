using DevArena.Web.Models.JudgeApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DevArena.Web.Controllers
{
    [Authorize(Roles = "Judge")]
    public class JudgeDashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public JudgeDashboardController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private HttpClient CreateClient()
        {
            var baseUrl = _configuration["JudgeApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("JudgeApi:BaseUrl is missing in appsettings.json.");
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            return client;
        }

        private string? GetJudgeId()
        {
            return User.FindFirst("JudgeId")?.Value;
        }

        public async Task<IActionResult> Index()
        {
            var judgeId = GetJudgeId();

            if (string.IsNullOrEmpty(judgeId))
            {
                TempData["Error"] = "Judge ID not found. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            var client = CreateClient();

            var response = await client.GetAsync($"api/judge/{judgeId}/contests");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load assigned contests. Make sure DevArena.JudgeApi is running.";
                return View(new List<JudgeContestDto>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var contests = JsonSerializer.Deserialize<List<JudgeContestDto>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            ViewBag.JudgeId = judgeId;
            ViewBag.JudgeName = User.Identity?.Name;

            return View(contests ?? new List<JudgeContestDto>());
        }

        public async Task<IActionResult> Profile()
        {
            var judgeId = GetJudgeId();

            if (string.IsNullOrEmpty(judgeId))
            {
                TempData["Error"] = "Judge ID not found. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            var client = CreateClient();

            var response = await client.GetAsync($"api/judge/{judgeId}/profile");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load judge profile.";
                return RedirectToAction("Index", "JudgeDashboard");
            }

            var json = await response.Content.ReadAsStringAsync();

            var profile = JsonSerializer.Deserialize<JudgeProfileDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (profile == null)
            {
                TempData["Error"] = "Invalid profile response.";
                return RedirectToAction("Index", "JudgeDashboard");
            }

            return View(profile);
        }
    }
}