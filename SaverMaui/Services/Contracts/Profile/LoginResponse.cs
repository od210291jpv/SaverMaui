using Newtonsoft.Json;

using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Content;

namespace SaverMaui.Services.Contracts.Profile
{
    internal class LoginResponse
    {
        [JsonProperty("profileId")]
        public Guid ProfileId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("publishedCategories")]
        public object PublishedCategories { get; set; }

        [JsonProperty("publications")]
        public List<ContentDto> Publications { get; set; }

        [JsonProperty("favoriteCategories")]
        public List<CategoryDto> FavoriteCategories { get; set; }

        [JsonProperty("friends")]
        public object Friends { get; set; }

        [JsonProperty("groups")]
        public object Groups { get; set; }

        [JsonProperty("isOnline")]
        public bool IsOnline { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }
    }
}
