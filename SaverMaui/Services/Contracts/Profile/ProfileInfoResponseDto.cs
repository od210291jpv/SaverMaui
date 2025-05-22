using Newtonsoft.Json;

namespace SaverMaui.Services.Contracts.Profile
{
    public class ProfileInfoResponseDto
    {
        [JsonProperty("profileId")]
        public string ProfileId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("publishedCategories")]
        public object PublishedCategories { get; set; }

        [JsonProperty("publications")]
        public List<object> Publications { get; set; }

        [JsonProperty("favoriteCategories")]
        public List<object> FavoriteCategories { get; set; }

        [JsonProperty("friends")]
        public List<object> Friends { get; set; }

        [JsonProperty("groups")]
        public List<object> Groups { get; set; }

        [JsonProperty("isOnline")]
        public bool IsOnline { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }

        [JsonProperty("funds")]
        public decimal Funds { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
