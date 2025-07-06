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

        public ContentServiceAction ContentActions 
        { 
            get => new ContentServiceAction(this.restClient, this.BaseUrl);
        }

        public SaverBackendClient(SaverBackendClientSettings settings)
        {
            RestClientOptions options = new RestClientOptions(settings.BaseUrl)
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
                Timeout = settings.Timeout
            };

            this.BaseUrl = settings.BaseUrl;
            this.restClient = new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson());
        }
    }
}
