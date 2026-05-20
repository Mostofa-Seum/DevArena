using System;
using System.Linq;
using DevArena.Data;
using DevArena.Entities;
using DevArena.Shared;

namespace DevArena.Repos
{
    public class JudgeRepo(DevArenaDbContext context)
    {
        public Result<object> GetAssignedContests(int judgeId)
        {
            var result = new Result<object>();

            try
            {
                var judgeExists = context.Judges
                    .Any(j => j.id == judgeId && j.Is_active == true);

                if (!judgeExists)
                {
                    result.HasError = true;
                    result.Message = "Judge not found or inactive.";
                    return result;
                }

                result.Data = context.Judges
                    .Where(j => j.id == judgeId && j.Is_active == true)
                    .Join(
                        context.Contests,
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
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<object> GetJudgeProfile(int judgeId)
        {
            var result = new Result<object>();

            try
            {
                var profile = context.Judges
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
                    .FirstOrDefault();

                if (profile == null)
                {
                    result.HasError = true;
                    result.Message = "Judge profile not found.";
                    return result;
                }

                result.Data = profile;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<object> GetContestDetails(int judgeId, int contestId)
        {
            var result = new Result<object>();

            try
            {
                if (!IsJudgeAssignedToContest(judgeId, contestId))
                {
                    result.HasError = true;
                    result.Message = "This judge is not assigned to this contest.";
                    return result;
                }

                var contest = context.Contests
                    .Where(c => c.id == contestId)
                    .Select(c => new
                    {
                        c.id,
                        c.title,
                        c.start_time,
                        c.end_time,
                        c.is_active
                    })
                    .FirstOrDefault();

                if (contest == null)
                {
                    result.HasError = true;
                    result.Message = "Contest not found.";
                    return result;
                }

                var problems = context.Problems
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
                    .ToList();

                result.Data = new
                {
                    contest,
                    problems
                };
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Problems> AddProblem(int judgeId, int contestId, AddProblemRequest request)
        {
            var result = new Result<Problems>();

            try
            {
                if (request == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid request body.";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    result.HasError = true;
                    result.Message = "Problem title is required.";
                    return result;
                }

                if (!IsJudgeAssignedToContest(judgeId, contestId))
                {
                    result.HasError = true;
                    result.Message = "This judge is not assigned to this contest.";
                    return result;
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

                context.Problems.Add(problem);
                context.SaveChanges();

                result.Data = problem;
                result.Message = "Problem added successfully.";
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<object> GetSubmissionsByContest(int judgeId, int contestId)
        {
            var result = new Result<object>();

            try
            {
                if (!IsJudgeAssignedToContest(judgeId, contestId))
                {
                    result.HasError = true;
                    result.Message = "This judge is not assigned to this contest.";
                    return result;
                }

                result.Data = context.Submissions
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
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<object> GetSubmissionDetails(int judgeId, int submissionId)
        {
            var result = new Result<object>();

            try
            {
                var submission = context.Submissions
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
                    .FirstOrDefault();

                if (submission == null)
                {
                    result.HasError = true;
                    result.Message = "Submission not found.";
                    return result;
                }

                if (!IsJudgeAssignedToContest(judgeId, submission.contest_id))
                {
                    result.HasError = true;
                    result.Message = "This judge cannot review this submission.";
                    return result;
                }

                result.Data = submission;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Review> ReviewSubmission(int judgeId, int submissionId, ReviewSubmissionRequest request)
        {
            var result = new Result<Review>();

            try
            {
                if (request == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid request body.";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(request.Verdict))
                {
                    result.HasError = true;
                    result.Message = "Verdict is required.";
                    return result;
                }

                if (request.Verdict != "Accepted" && request.Verdict != "Rejected")
                {
                    result.HasError = true;
                    result.Message = "Verdict must be either Accepted or Rejected.";
                    return result;
                }

                var submission = context.Submissions
                    .FirstOrDefault(s => s.id == submissionId);

                if (submission == null)
                {
                    result.HasError = true;
                    result.Message = "Submission not found.";
                    return result;
                }

                if (!IsJudgeAssignedToContest(judgeId, submission.contest_id))
                {
                    result.HasError = true;
                    result.Message = "This judge cannot review this submission.";
                    return result;
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

                context.Reviews.Add(review);
                context.Submissions.Update(submission);
                context.SaveChanges();

                result.Data = review;
                result.Message = "Submission reviewed successfully.";
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<object> GetReviewsByJudge(int judgeId)
        {
            var result = new Result<object>();

            try
            {
                var judgeExists = context.Judges
                    .Any(j => j.id == judgeId && j.Is_active == true);

                if (!judgeExists)
                {
                    result.HasError = true;
                    result.Message = "Judge not found or inactive.";
                    return result;
                }

                result.Data = context.Reviews
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
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        private bool IsJudgeAssignedToContest(int judgeId, int contestId)
        {
            return context.Judges.Any(j =>
                j.id == judgeId &&
                j.contest_id == contestId &&
                j.Is_active == true
            );
        }
    }

    public class AddProblemRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string InputFormat { get; set; } = string.Empty;
        public string OutputFormat { get; set; } = string.Empty;
    }

    public class ReviewSubmissionRequest
    {
        public string Verdict { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
    }
}