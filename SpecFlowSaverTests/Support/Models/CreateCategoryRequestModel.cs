using Newtonsoft.Json;
using SpecFlowSaverTests.Services.Interfaces;

namespace SpecFlowSaverTests.Support.Models
{
    internal class CreateCategoryRequestModel : IRequest
    {
        public CreateCategoryRequestModel()
        {
            this.CategoryId = Guid.NewGuid();
            this.AmountOfOpenings = 0;
            this.AmountOfFavorites = 0;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonProperty("amountOfOpenings")]
        public int AmountOfOpenings { get; set; }

        [JsonProperty("amountOfFavorites")]
        public int AmountOfFavorites { get; set; }
    }
}
