
namespace SaverBackendApiClient.Configuration
{
    public class SaverBackendClientSettings
    {
        public string BaseUrl { get; internal set; } = string.Empty;

        public TimeSpan? Timeout { get; internal set; }
    }
}
