using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using SaverBackendApiClient.Configuration;
using SaverBackendApiClient.ServiceActions;

namespace SaverBackendApiClient
{
    public class SaverBackendClient
    {
        public string BaseUrl { get; }

        private RestClient restClient;

        public UserProfileServiceActions ProfileActions 
        { 
            get => new UserProfileServiceActions(this.restClient, this.BaseUrl);
        }

        public SaverBackendClient(IOptionsMonitor<SaverBackendClientSettings> settings)
        {
            RestClientOptions options = new RestClientOptions(settings.CurrentValue.BaseUrl)
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
                Timeout = settings.CurrentValue.Timeout
            };

            this.BaseUrl = settings.CurrentValue.BaseUrl;
            this.restClient = new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson());
        }
    }
}
