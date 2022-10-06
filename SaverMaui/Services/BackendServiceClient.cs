using Newtonsoft.Json;

using SaverMaui.Services.Interfaces;

using System.Text;

namespace SaverMaui.Services
{
    public class BackendServiceClient : IHttpServiceClient
    {
        private static BackendServiceClient Instance;

        private readonly HttpClient httpClient;
        private readonly string hostIp;

        private BackendServiceClient(string hostIp)
        {
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            this.hostIp = hostIp;
        }

        public static BackendServiceClient GetInstance(string hostIp = "localhost")
        {
            if (Instance == null)
            {
                Instance = new BackendServiceClient(hostIp);
            }

            return Instance;
        }

        public async Task<HttpResponseMessage> GetRequestAsync(string url)
        {
            return await this.httpClient.GetAsync(url);
        }

        public async Task<HttpResponseMessage> PostRequestAsync(string endpoint, IRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, encoding: Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var resp = await client.PostAsync(endpoint, data);

            return resp;
        }

        public async Task<HttpResponseMessage> PostRequestAsync(string endpoint) 
        {
            var client = new HttpClient();
            var resp = await client.PostAsync(endpoint, null);

            return resp;
        }
    }
}
