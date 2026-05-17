using DevArena.Data;
using DevArena.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevArena.JudgeApi.Controllers
{
    [ApiController]
    [Route("api/judge")]
    public class JudgeController : ControllerBase
    {
        private readonly DevArenaDbContext _context;

        public JudgeController(DevArenaDbContext context)
        {
            _context = context;
        }

        // GET: api/judge/501/contests
        [HttpGet("{judgeId}/contests")]
        public async Task<IActionResult> GetAssignedContests(int judgeId)
        {
            var judgeExists = await _context.Judges
                .AnyAsync(j => j.id == judgeId && j.Is_active == true);

            if (!judgeExists)
            {
                return NotFound(new
                {
                    message = "Judge not found or inactive."
                });
            }

            var contests = await _context.Judges
                .Where(j => j.id == judgeId && j.Is_active == true)
                .Join(
                    _context.Contests,
                    judge => judge.contest_id,
                    contest => contest.id,
                    (judge, contest) => new
                    {
                        contest.id,
                        contest.title,
                        contest.start_time,
                        contest.end_time,
                        contest.is_active
                    }
                )
                .ToListAsync();

            return Ok(contests);
        }

        // GET: api/judge/501/profile
        [HttpGet("{judgeId}/profile")]
        public async Task<IActionResult> GetJudgeProfile(int judgeId)
        {
            var profile = await _context.Judges
                .Where(j => j.id == judgeId && j.Is_active == true)
                .Select(j => new
                {
                    name = j.Participants.name,
                    email = j.Participants.email,
                    contestTitle = j.Contest.title,
                    promotedByHostName = j.Host.name,
                    isActive = j.Is_active,
                    createdAt = j.created_at
                })
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Judge profile not found."
                });
            }

            return Ok(profile);
        }

        // GET: api/judge/501/contests/601
        [HttpGet("{judgeId}/contests/{contestId}")]
        public async Task<IActionResult> GetContestDetails(int judgeId, int contestId)
        {
            var isAssigned = await IsJudgeAssignedToContest(judgeId, contestId);

            if (!isAssigned)
            {
                return Unauthorized(new
                {
                    message = "This judge is not assigned to this contest."
                });
            }

            var contest = await _context.Contests
                .Where(c => c.id == contestId)
                .Select(c => new
                {
                    c.id,
                    c.title,
                    c.start_time,
                    c.end_time,
                    c.is_active
                })
                .FirstOrDefaultAsync();

            if (contest == null)
            {
                return NotFound(new
                {
                    message = "Contest not found."
                });
            }

            var problems = await _context.Problems
                .Where(p => p.contest_id == contestId)
                .Select(p => new
                {
                    p.problem_id,
                    p.title,
                    p.description,
                    p.input_format,
                    p.output_format,
                    p.judge_id,
                    p.contest_id
                })
                .ToListAsync();

            return Ok(new
            {
                contest,
                problems
            });
        }

        // POST: api/judge/501/contests/601/problems
        [HttpPost("{judgeId}/contests/{contestId}/problems")]
        public async Task<IActionResult> AddProblem(
            int judgeId,
            int contestId,
            [FromBody] AddProblemRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Invalid request body."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new
                {
                    message = "Problem title is required."
                });
            }

            var isAssigned = await IsJudgeAssignedToContest(judgeId, contestId);

            if (!isAssigned)
            {
                return Unauthorized(new
                {
                    message = "This judge is not assigned to this contest."
                });
            }

            var problem = new Problems
            {
                title = request.Title,
                description = request.Description,
                input_format = request.InputFormat,
                output_format = request.OutputFormat,
                judge_id = judgeId,
                contest_id = contestId
            };

            _context.Problems.Add(problem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Problem added successfully.",
                problem
            });
        }

        // GET: api/judge/501/contests/601/submissions
        [HttpGet("{judgeId}/contests/{contestId}/submissions")]
        public async Task<IActionResult> GetSubmissionsByContest(int judgeId, int contestId)
        {
            var isAssigned = await IsJudgeAssignedToContest(judgeId, contestId);

            if (!isAssigned)
            {
                return Unauthorized(new
                {
                    message = "This judge is not assigned to this contest."
                });
            }

            var submissions = await _context.Submissions
                .Where(s => s.contest_id == contestId)
                .OrderByDescending(s => s.submitted_at)
                .Select(s => new
                {
                    s.id,
                    s.contest_id,
                    contestTitle = s.Contest.title,
                    participantName = s.Participant.name,
                    problemTitle = s.Problem.title,
                    s.code_text,
                    s.status,
                    s.submitted_at
                })
                .ToListAsync();

            return Ok(submissions);
        }

        // GET: api/judge/501/submissions/1102
        [HttpGet("{judgeId}/submissions/{submissionId}")]
        public async Task<IActionResult> GetSubmissionDetails(int judgeId, int submissionId)
        {
            var submission = await _context.Submissions
                .Where(s => s.id == submissionId)
                .Select(s => new
                {
                    s.id,
                    s.contest_id,
                    contestTitle = s.Contest.title,
                    participantName = s.Participant.name,
                    problemTitle = s.Problem.title,
                    s.code_text,
                    s.status,
                    s.submitted_at
                })
                .FirstOrDefaultAsync();

            if (submission == null)
            {
                return NotFound(new
                {
                    message = "Submission not found."
                });
            }

            var isAssigned = await IsJudgeAssignedToContest(judgeId, submission.contest_id);

            if (!isAssigned)
            {
                return Unauthorized(new
                {
                    message = "This judge cannot review this submission."
                });
            }

            return Ok(submission);
        }

        // POST: api/judge/501/submissions/1102/review
        [HttpPost("{judgeId}/submissions/{submissionId}/review")]
        public async Task<IActionResult> ReviewSubmission(
            int judgeId,
            int submissionId,
            [FromBody] ReviewSubmissionRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Invalid request body."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Verdict))
            {
                return BadRequest(new
                {
                    message = "Verdict is required."
                });
            }

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.id == submissionId);

            if (submission == null)
            {
                return NotFound(new
                {
                    message = "Submission not found."
                });
            }

            var isAssigned = await IsJudgeAssignedToContest(judgeId, submission.contest_id);

            if (!isAssigned)
            {
                return Unauthorized(new
                {
                    message = "This judge cannot review this submission."
                });
            }

            var review = new Review
            {
                submission_id = submissionId,
                judge_id = judgeId,
                verdict = request.Verdict,
                feedback = request.Feedback,
                reviewed_at = DateTime.Now
            };

            submission.status = request.Verdict;

            _context.Reviews.Add(review);
            _context.Submissions.Update(submission);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Submission reviewed successfully.",
                review
            });
        }

        // GET: api/judge/501/reviews
        [HttpGet("{judgeId}/reviews")]
        public async Task<IActionResult> GetReviewsByJudge(int judgeId)
        {
            var judgeExists = await _context.Judges
                .AnyAsync(j => j.id == judgeId && j.Is_active == true);

            if (!judgeExists)
            {
                return NotFound(new
                {
                    message = "Judge not found or inactive."
                });
            }

            var reviews = await _context.Reviews
                .Where(r => r.judge_id == judgeId)
                .OrderByDescending(r => r.reviewed_at)
                .Select(r => new
                {
                    r.id,
                    r.submission_id,
                    r.judge_id,
                    r.verdict,
                    r.feedback,
                    r.reviewed_at
                })
                .ToListAsync();

            return Ok(reviews);
        }

        private async Task<bool> IsJudgeAssignedToContest(int judgeId, int contestId)
        {
            return await _context.Judges.AnyAsync(j =>
                j.id == judgeId &&
                j.contest_id == contestId &&
                j.Is_active == true
            );
        }
    }

    public class AddProblemRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
    }

    public class ReviewSubmissionRequest
    {
        public string Verdict { get; set; }
        public string Feedback { get; set; }
    }
}