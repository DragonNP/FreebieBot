using FreebieBot.Models.EventLogs;
using FreebieBot.Models.Posts;
using FreebieBot.Models.Translates;
using FreebieBot.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace FreebieBot.Models.Database
{
    public sealed class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } // Database for users
        public DbSet<Line> Lines { get; set; } // Database for translates
        public DbSet<EventLog> EventLogs { get; set; } // Database for Event Logs
        public DbSet<Post> Posts { get; set; } // Database for freebie
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}