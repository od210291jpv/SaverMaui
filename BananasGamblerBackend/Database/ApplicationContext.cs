using BananasGamblerBackend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BananasGamblerBackend.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<GameCard> GameCards { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(c => c.Cards)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .IsRequired(false);
        }
    }
}
