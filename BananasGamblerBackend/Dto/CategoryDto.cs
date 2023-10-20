using Newtonsoft.Json;

namespace BananasGamblerBackend.Dto
{
    public class CategoryDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonProperty("amountOfOpenings")]
        public int? AmountOfOpenings { get; set; }

        [JsonProperty("amountOfFavorites")]
        public int? AmountOfFavorites { get; set; }

        [JsonProperty("PublisherProfileId")]
        public Guid? PublisherProfileId { get; set; }

        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
    }
}
