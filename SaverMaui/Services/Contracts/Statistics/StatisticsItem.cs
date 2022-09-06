using Newtonsoft.Json;

namespace SaverMaui.Services.Contracts.Statistics
{
    public class StatisticsItem
    {
        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonProperty("openings")]
        public int? Openings { get; set; }

        [JsonProperty("favorites")]
        public int? Favorites { get; set; }
    }
}