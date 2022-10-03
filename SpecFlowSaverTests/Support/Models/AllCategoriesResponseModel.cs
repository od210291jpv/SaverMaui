using Newtonsoft.Json;

namespace SpecFlowSaverTests.Support.Models
{
    internal class AllCategoriesResponseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonProperty("amountOfOpenings")]
        public int? AmountOfOpenings { get; set; }

        [JsonProperty("amountOfFavorites")]
        public int? AmountOfFavorites { get; set; }
    }
}
