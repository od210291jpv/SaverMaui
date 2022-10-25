using Newtonsoft.Json;

namespace SaverMaui.Services.Contracts.Profile
{
    public class PublishedCategory
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("amountOfOpenings")]
        public int AmountOfOpenings { get; set; }

        [JsonProperty("amountOfFavorites")]
        public int AmountOfFavorites { get; set; }

        [JsonProperty("PublisherProfileId")]
        public string PublisherProfileId { get; set; }
    }
}
