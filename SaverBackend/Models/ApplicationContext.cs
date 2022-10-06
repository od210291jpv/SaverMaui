using Microsoft.EntityFrameworkCore;

namespace SaverBackend.Models
{
    public class ApplicationContext : DbContext
    {
        //public static string ConnectionString = "Database=mobilesdb;Uid=user;Pwd=password;";
        public static string ConnectionString = "Database=mobilesdb;Uid=root;Pwd=password;";

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
    }
}
