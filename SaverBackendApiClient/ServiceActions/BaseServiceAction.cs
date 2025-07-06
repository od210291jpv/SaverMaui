using RestSharp;

namespace SaverBackendApiClient.ServiceActions
{
    public class BaseServiceAction
    {
        protected RestClient Client { get; private set; }
        public Uri BaseUrl { get; }

        public BaseServiceAction(RestClient client, string baseUrl)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client), "Client cannot be null.");
            this.BaseUrl = new Uri(baseUrl ?? throw new ArgumentNullException(nameof(baseUrl), "Base URL cannot be null."));
        }
    }
}
