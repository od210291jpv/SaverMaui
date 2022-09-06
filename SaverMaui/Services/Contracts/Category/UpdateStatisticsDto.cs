using Newtonsoft.Json;
using SaverMaui.Services.Interfaces;

namespace SaverMaui.Services.Contracts.Category
{
    public class UpdateStatisticsDto : IRequest
    {
        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonProperty("openings")]
        public int Openings { get; set; }

        [JsonProperty("favorites")]
        public int Favorites { get; set; }
    }
}
