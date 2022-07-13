using FinancialChat.Common;
using FinancialChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.Infrastructure.Persistence
{
    public class FinancialChatDbContext : DbContext
    {
        public FinancialChatDbContext(DbContextOptions<FinancialChatDbContext> options) : base(options) { }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasData(
                    new User(1, "user1", "password1".ToSha256()),
                    new User(2, "user2", "password2".ToSha256()),
                    new User(3, "user3", "password3".ToSha256()),
                    new User(4, "user4", "password4".ToSha256()));
        }
    }
}
