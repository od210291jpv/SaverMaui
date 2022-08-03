using Newtonsoft.Json;

namespace SaverMaui.Services.Contracts.Category
{
    public class CategoryDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }
    }
}
