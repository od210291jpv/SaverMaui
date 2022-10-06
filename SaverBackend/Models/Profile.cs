namespace SaverBackend.Models
{
    public class Profile
    {
        public int Id { get; set; }

        public Guid ProfileId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VerificationCode { get; set; }

        public List<Category> PublishedCategories { get; set; }

        public List<Content> Publications { get; set; }

        public List<Category> FavoriteCategories { get; set; }

        public List<Profile> Friends { get; set; }

        public List<Group> Groups { get; set; }

        public bool IsOnline { get; set; }
    }
}
