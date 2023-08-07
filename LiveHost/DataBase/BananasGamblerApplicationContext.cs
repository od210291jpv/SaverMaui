using LiveHost.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveHost.DataBase
{
    public class BananasGamblerApplicationContext : DbContext
    {
        public static string ConnectionString = string.Empty;

        public DbSet<User> Users { get; set; }

        public DbSet<GameCard> GameCards { get; set; }

        public BananasGamblerApplicationContext(DbContextOptions<ApplicationContext> options)
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
