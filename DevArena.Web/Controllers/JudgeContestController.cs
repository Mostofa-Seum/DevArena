using DevArena.Web.Models.JudgeApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DevArena.Web.Controllers
{
    [Authorize(Roles = "Judge")]
    public class JudgeContestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public JudgeContestController(
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

        public async Task<IActionResult> Details(int contestId)
        {
            var judgeId = GetJudgeId();

            if (string.IsNullOrEmpty(judgeId))
            {
                TempData["Error"] = "Judge ID not found. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            var client = CreateClient();

            var contestResponse = await client.GetAsync($"api/judge/{judgeId}/contests/{contestId}");

            if (!contestResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load contest details.";
                return RedirectToAction("Index", "JudgeDashboard");
            }

            var contestJson = await contestResponse.Content.ReadAsStringAsync();

            var contestDetails = JsonSerializer.Deserialize<ContestDetailsResponse>(
                contestJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (contestDetails == null)
            {
                TempData["Error"] = "Invalid contest details response.";
                return RedirectToAction("Index", "JudgeDashboard");
            }

            var submissionResponse = await client.GetAsync($"api/judge/{judgeId}/contests/{contestId}/submissions");

            var submissions = new List<JudgeSubmissionDto>();

            if (submissionResponse.IsSuccessStatusCode)
            {
                var submissionJson = await submissionResponse.Content.ReadAsStringAsync();

                submissions = JsonSerializer.Deserialize<List<JudgeSubmissionDto>>(
                    submissionJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<JudgeSubmissionDto>();
            }

            ViewBag.JudgeId = judgeId;
            ViewBag.Submissions = submissions;

            return View(contestDetails);
        }

        [HttpGet]
        public IActionResult AddProblem(int contestId)
        {
            ViewBag.ContestId = contestId;
            return View(new AddProblemRequest());
        }

        [HttpPost]
        public async Task<IActionResult> AddProblem(int contestId, AddProblemRequest model)
        {
            var judgeId = GetJudgeId();

            if (string.IsNullOrEmpty(judgeId))
            {
                TempData["Error"] = "Judge ID not found. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("Title", "Problem title is required.");
                ViewBag.ContestId = contestId;
                return View(model);
            }

            var client = CreateClient();

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                $"api/judge/{judgeId}/contests/{contestId}/problems",
                content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not add problem.";
                ViewBag.ContestId = contestId;
                return View(model);
            }

            TempData["Success"] = "Problem added successfully.";
            return RedirectToAction("Details", new { contestId });
        }

        [HttpGet]
        public async Task<IActionResult> ReviewSubmission(int submissionId)
        {
            var judgeId = GetJudgeId();

            if (string.IsNullOrEmpty(judgeId))
            {
                TempData["Error"] = "Judge ID not found. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            var client = CreateClient();

            var response = await client.GetAsync($"api/judge/{judgeId}/submissions/{submissionId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load submission.";
                return RedirectToAction("Index", "JudgeDashboard");
            }

            var json = await response.Content.ReadAsStringAsync();

            var submission = JsonSerializer.Deserialize<JudgeSubmissionDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (submission == null)
            {
                TempData["Error"] = "Invalid submission response.";
                return RedirectToAction("Index", "JudgeDashboard");
            }

            return View(submission);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewSubmission(
            int submissionId,
            string verdict,
            string feedback)
        {
            var judgeId = GetJudgeId();

            if (string.IsNullOrEmpty(judgeId))
            {
                TempData["Error"] = "Judge ID not found. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrWhiteSpace(verdict))
            {
                TempData["Error"] = "Verdict is required.";
                return RedirectToAction("ReviewSubmission", new { submissionId });
            }

            if (verdict != "Accepted" && verdict != "Rejected")
            {
                TempData["Error"] = "Verdict must be either Accepted or Rejected.";
                return RedirectToAction("ReviewSubmission", new { submissionId });
            }

            var request = new ReviewSubmissionRequest
            {
                Verdict = verdict,
                Feedback = feedback ?? string.Empty
            };

            var client = CreateClient();

            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                $"api/judge/{judgeId}/submissions/{submissionId}/review",
                content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not submit review.";
                return RedirectToAction("ReviewSubmission", new { submissionId });
            }

            TempData["Success"] = "Submission reviewed successfully.";
            return RedirectToAction("Index", "JudgeDashboard");
        }
    }
}