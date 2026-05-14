using Microsoft.EntityFrameworkCore;
using DevArena.Entities;
namespace DevArena.Data
{
    public class DevArenaDbContext:DbContext
    {
        public DevArenaDbContext(DbContextOptions<DevArenaDbContext> options) : base(options)
        {
        }
        public DbSet<Contests> Contests { get; set; }
        public DbSet<Participants> Participants { get; set; }
        public DbSet<Problems> Problems { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Judge> Judges { get; set; }
        public DbSet<Host> Hosts { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ContestRegistration> ContestRegistrations { get; set; }
    }
}
