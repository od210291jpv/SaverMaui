using Newtonsoft.Json;

namespace SaverMaui.Services.Contracts.Content
{
    public class ContentDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; }

        [JsonProperty("categoryId")]
        public Guid? CategoryId { get; set; }
    }
}
