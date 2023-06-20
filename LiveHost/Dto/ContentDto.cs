using Newtonsoft.Json;

namespace LiveHost.Dto
{
    public class ContentDto
    {
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; } = string.Empty;

        [JsonProperty("categoryId")]
        public Guid? CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}