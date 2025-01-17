using Newtonsoft.Json;
using SaverMaui.Services.Interfaces;

namespace SaverMaui.Services.Contracts
{
    public class GetAllContentResponseModel : IResponse
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; }

        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        public int Id { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        public short Rating { get; set; } = 0;

        public decimal Cost { get; set; } = 0m;
    }
}
