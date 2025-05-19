using Microsoft.EntityFrameworkCore;
using GorevYoneticiAPI.Models;

namespace GorevYoneticiAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskReport> TaskReports { get; set; }
    }
}
