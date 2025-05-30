﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SaverBackend.Models
{
    public class Profile
    {
        public int Id { get; set; }

        public Guid ProfileId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string VerificationCode { get; set; } = string.Empty;

        public List<Category> PublishedCategories { get; set; } = new List<Category>();

        public List<Content> Publications { get; set; } = new List<Content>();

        public List<Category> FavoriteCategories { get; set; } = new List<Category>();

        public List<VideoContent> VideoContents { get; set; } = new List<VideoContent>();

        public List<Profile> Friends { get; set; } = new List<Profile>();

        public List<Group> Groups { get; set; } = new List<Group>();

        public bool IsOnline { get; set; }

        public ICollection<Content> FavoriteContent { get; } = new List<Content>();

        public decimal Funds { get; set; } = 0.0m;
    }
}
