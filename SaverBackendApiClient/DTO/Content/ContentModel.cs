using Newtonsoft.Json;

namespace SaverBackendApiClient.DTO.Content
{
    public class ContentModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("profileId")]
        public object ProfileId { get; set; }

        [JsonProperty("profile")]
        public List<object> Profile { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("cost")]
        public decimal Cost { get; set; }
    }
}
