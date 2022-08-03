using Newtonsoft.Json;

using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Content;
using SaverMaui.Services.Interfaces;

namespace SaverMaui.Services.Contracts
{
    public class ContentRepresentationData : IRequest
    {
        [JsonProperty("categories")]
        public CategoryDto[] Categories { get; set; }

        [JsonProperty("content")]
        public ContentDto[] Content { get; set; }
    }
}
