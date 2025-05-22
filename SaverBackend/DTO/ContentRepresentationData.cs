using Newtonsoft.Json;

namespace SaverBackend.DTO
{
    public class ContentRepresentationData
    {
        [JsonProperty("categories")]
        public CategoryDto[] Categories { get; set; } = Array.Empty<CategoryDto>();

        [JsonProperty("content")]
        public ContentDto[] Content { get; set; } = Array.Empty<ContentDto>();
    }
}
