
namespace SaverBackendApiClient.Configuration
{
    public class SaverBackendClientSettings
    {
        public string BaseUrl { get;  set; } = string.Empty;

        public TimeSpan? Timeout { get;  set; }
    }
}
