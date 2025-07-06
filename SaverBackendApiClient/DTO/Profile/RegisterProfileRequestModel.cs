using Newtonsoft.Json;

namespace SaverBackendApiClient.DTO.Profile
{
    public class RegisterProfileRequestModel
    {
        [JsonProperty("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;

        [JsonProperty("verificationCode")]
        public string VerificationCode { get; set; } = string.Empty;
    }
}
