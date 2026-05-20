using DevArena.Repos;
using Microsoft.AspNetCore.Mvc;

namespace DevArena.JudgeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JudgeController(JudgeRepo repo) : ControllerBase
    {
        [HttpGet("{judgeId}/contests")]
        public IActionResult GetAssignedContests(int judgeId)
        {
            var result = repo.GetAssignedContests(judgeId);

            if (result.HasError)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("{judgeId}/profile")]
        public IActionResult GetJudgeProfile(int judgeId)
        {
            var result = repo.GetJudgeProfile(judgeId);

            if (result.HasError)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("{judgeId}/contests/{contestId}")]
        public IActionResult GetContestDetails(int judgeId, int contestId)
        {
            var result = repo.GetContestDetails(judgeId, contestId);

            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPost("{judgeId}/contests/{contestId}/problems")]
        public IActionResult AddProblem(
            int judgeId,
            int contestId,
            [FromBody] AddProblemRequest request)
        {
            var result = repo.AddProblem(judgeId, contestId, request);

            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                problem = result.Data
            });
        }

        [HttpGet("{judgeId}/contests/{contestId}/submissions")]
        public IActionResult GetSubmissionsByContest(int judgeId, int contestId)
        {
            var result = repo.GetSubmissionsByContest(judgeId, contestId);

            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("{judgeId}/submissions/{submissionId}")]
        public IActionResult GetSubmissionDetails(int judgeId, int submissionId)
        {
            var result = repo.GetSubmissionDetails(judgeId, submissionId);

            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPost("{judgeId}/submissions/{submissionId}/review")]
        public IActionResult ReviewSubmission(
            int judgeId,
            int submissionId,
            [FromBody] ReviewSubmissionRequest request)
        {
            var result = repo.ReviewSubmission(judgeId, submissionId, request);

            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                review = result.Data
            });
        }

        [HttpGet("{judgeId}/reviews")]
        public IActionResult GetReviewsByJudge(int judgeId)
        {
            var result = repo.GetReviewsByJudge(judgeId);

            if (result.HasError)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }
    }
}