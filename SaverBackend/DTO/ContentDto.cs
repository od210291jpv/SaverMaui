using Newtonsoft.Json;

namespace SaverBackend.DTO
{
    public class ContentDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; } = string.Empty;

        [JsonProperty("categoryId")]
        public Guid? CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
