using Newtonsoft.Json;

namespace SaverMaui.Services.Contracts.Content
{
    public class GetFavoriteContentRequestDto
    {
        [JsonProperty("login")]
        public string Logn { get; set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }
}
