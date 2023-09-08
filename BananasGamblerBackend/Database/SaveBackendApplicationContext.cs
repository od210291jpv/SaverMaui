using BananasGamblerBackend.Database.SaverBackendModels;
using Microsoft.EntityFrameworkCore;

namespace BananasGamblerBackend.Database
{
    public class SaveBackendApplicationContext : DbContext
    {
        public static string ConnectionString = string.Empty;

        public DbSet<Category> Categories { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<Group> Groups { get; set; }



        public SaveBackendApplicationContext(DbContextOptions<SaveBackendApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
