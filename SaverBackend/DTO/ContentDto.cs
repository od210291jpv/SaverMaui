using Newtonsoft.Json;

namespace SaverBackend.DTO
{
    public class ContentDto
    {
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; } = string.Empty;

        [JsonProperty("categoryId")]
        public Guid? CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int? ProfileId { get; set; }
 
        public object? Profile { get; set; }

        public short Rating { get; set; } = 0;

        public decimal Cost { get; set; } = 0.0m;
    }
}
