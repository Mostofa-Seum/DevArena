namespace DevArena.Web.Models.JudgeApi
{
    public class JudgeContestDto
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public bool Is_active { get; set; }
    }

    public class JudgeProblemDto
    {
        public int problem_id { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string input_format { get; set; } = string.Empty;
        public string output_format { get; set; } = string.Empty;
        public int judge_id { get; set; }
        public int contest_id { get; set; }
    }

    public class JudgeSubmissionDto
    {
        public int id { get; set; }

        // Keep contest_id internally for Back button/routing
        public int contest_id { get; set; }

        public string contestTitle { get; set; } = string.Empty;
        public string participantName { get; set; } = string.Empty;
        public string problemTitle { get; set; } = string.Empty;
        public string code_text { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public DateTime submitted_at { get; set; }
    }

    public class JudgeReviewDto
    {
        public int id { get; set; }
        public int submission_id { get; set; }
        public int judge_id { get; set; }
        public string verdict { get; set; } = string.Empty;
        public string feedback { get; set; } = string.Empty;
        public DateTime reviewed_at { get; set; }
    }

    public class JudgeProfileDto
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string contestTitle { get; set; } = string.Empty;
        public string promotedByHostName { get; set; } = string.Empty;
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class ContestDetailsResponse
    {
        public JudgeContestDto contest { get; set; } = new JudgeContestDto();
        public List<JudgeProblemDto> problems { get; set; } = new List<JudgeProblemDto>();
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