﻿using Microsoft.EntityFrameworkCore;

namespace SaverBackend.Models
{
    public class ApplicationContext : DbContext
    {
        public static string ConnectionString = "Server=192.168.88.252;Database=mobilesdb2;Uid=user;Pwd=password;";

        public DbSet<Category> Categories { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<ImageModel> Images { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<VideoContent> Videos { get; set; }

        public DbSet<ContentProfile> FavoriteContent { get; set; }

        public DbSet<BrokenContent> BrokenContents { get; set; }

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

            modelBuilder.Entity<Content>()
                .HasMany(e => e.Profile)
                .WithMany(p => p.FavoriteContent)
                .UsingEntity<ContentProfile>();
            //.HasForeignKey(p => p.ProfileId)
            //.IsRequired(false);

            modelBuilder.Entity<Profile>().HasMany(p => p.Publications);

            modelBuilder.Entity<Profile>()
                .HasMany(e => e.VideoContents)
                .WithOne(p => p.Profile)
                .HasForeignKey(p => p.ProfileId)
                .IsRequired(false);
        }
    }
}
