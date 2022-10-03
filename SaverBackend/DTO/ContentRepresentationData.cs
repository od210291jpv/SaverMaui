using Newtonsoft.Json;

namespace SaverBackend.DTO
{
    public class ContentRepresentationData
    {
        [JsonProperty("categories")]
        public CategoryDto[] Categories { get; set; }

        [JsonProperty("content")]
        public ContentDto[] Content { get; set; }
    }
}
