using Microsoft.EntityFrameworkCore;

namespace FreebieBot.Models.Database
{
    public sealed class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; } // Database for users
        public DbSet<Line> Lines { get; set; } // Database for translates
        public DbSet<EventLog> EventLogs { get; set; } // Database for Event Logs
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}