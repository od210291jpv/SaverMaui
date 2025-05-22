using Newtonsoft.Json;

namespace SaverBackend.DTO
{
    public class ContentDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; } = string.Empty;

        [JsonProperty("categoryId")]
        public Guid? CategoryId { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [JsonProperty("profileId")]
        public int? ProfileId { get; set; }

        [JsonProperty("profile")]
        public object? Profile { get; set; }

        [JsonProperty("rating")]
        public short Rating { get; set; } = 0;

        [JsonProperty("cost")]
        public decimal Cost { get; set; } = 0.0m;
    }
}
