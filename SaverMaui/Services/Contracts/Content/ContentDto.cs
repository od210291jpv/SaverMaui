using Newtonsoft.Json;

namespace SaverMaui.Services.Contracts.Content
{
    public class ContentDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; }

        [JsonProperty("categoryId")]
        public Guid? CategoryId { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("rating")]
        public short Rating { get; set; } = 0;

        [JsonProperty("cost")]
        public decimal Cost { get; set; } = 0.0m;
    }
}
