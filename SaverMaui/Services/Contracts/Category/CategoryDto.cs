using Newtonsoft.Json;

namespace SaverMaui.Services.Contracts.Category
{
    public class CategoryDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonProperty("amountOfOpenings")]
        public int? AmountOfOpenings { get; set; } = 0;

        [JsonProperty("amountOfFavorites")]
        public int? AmountOfFavorites { get; set; } = 0;

        [JsonProperty("PublisherProfileId")]
        public string PublisherProfileId { get; set; } = string.Empty;

        public DateTime TimeCreated { get; set; }
    }
}
