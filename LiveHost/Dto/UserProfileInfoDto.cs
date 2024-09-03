using SaverBackend.Models;

namespace LiveHost.Dto
{
    public class UserProfileInfoDto
    {
        public Guid ProfileId { get; set; }

        public string UserName { get; set; }

        public List<CategoryDto> PublishedCategories { get; set; } = new List<CategoryDto>();

        public List<Content> Publications { get; set; } = new List<Content>();

        public List<Category> FavoriteCategories { get; set; } = new List<Category>();

        public List<Profile> Friends { get; set; } = new List<Profile>();

        public List<Group> Groups { get; set; } = new List<Group>();

        public bool IsOnline { get; set; }

        public string? Error { get; set; }
    }
}
