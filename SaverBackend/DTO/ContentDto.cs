using Newtonsoft.Json;

namespace SaverBackend.DTO
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
