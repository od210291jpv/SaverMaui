using Newtonsoft.Json;

namespace SaverBackend.DTO
{
    public class UpdateStatisticsDto
    {
        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonProperty("openings")]
        public int Openings { get; set; }

        [JsonProperty("favorites")]
        public int Favorites { get; set; }
    }
}
