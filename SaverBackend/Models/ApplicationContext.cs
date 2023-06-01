using Microsoft.EntityFrameworkCore;

namespace SaverBackend.Models
{
    public class ApplicationContext : DbContext
    {
        public static string ConnectionString;

        public DbSet<Category> Categories { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<ImageModel> Images { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Group> Groups { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(p => p.Profile)
                .WithMany(t => t.PublishedCategories)
                .HasForeignKey(p => p.ProfileId);

            modelBuilder.Entity<Profile>()
                .HasMany(e => e.FavoriteContent)
                .WithOne(p => p.Profile)
                .HasForeignKey(p => p.ProfileId)
                .IsRequired(false);
        }
    }
}
