using System;
using Microsoft.EntityFrameworkCore;
using SOCRATIC_LEARNING_DOTNET.Entities;

namespace SOCRATIC_LEARNING_DOTNET.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> User { get; set; }
    }
}
