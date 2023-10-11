using BananasGamblerBackend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BananasGamblerBackend.Database
{
    public class ApplicationContext : DbContext
    {
        public static string ConnectionString = string.Empty;

        public DbSet<User> Users { get; set; }

        public DbSet<GameCard> GameCards { get; set; }

        public DbSet<CheckInCode> CheckInCodes { get; set; }
        public DbSet<GameCardUser> CardsToUsers { get; set; }

        public DbSet<Category> Categories { get; set; }

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
                .WithMany(p => p.Users);

            //modelBuilder.Entity<GameCard>().HasMany(c => c.Users).WithMany(c => c.Cards);
        }
    }
}
