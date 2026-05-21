using DevArena.Entities;
using System.Collections.Generic;

namespace DevArena.Web.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalAdmins { get; set; }
        public int TotalHosts { get; set; }
        public int TotalJudges { get; set; }
        public int TotalParticipants { get; set; }
        public int TotalContests { get; set; }
        public int TotalProblems { get; set; }
        public int TotalSubmissions { get; set; }

        public Admin? CurrentAdmin { get; set; }

        public List<Contests> RecentContests { get; set; } = new();
    }
}