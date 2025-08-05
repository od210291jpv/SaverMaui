using RestSharp;
using SaverBackendApiClient.DTO.Content;
using SaverBackendApiClient.Helpers;

namespace SaverBackendApiClient.ServiceActions
{
    public class ContentServiceAction : BaseServiceAction
    {
        public ContentServiceAction(RestClient client, string baseUrl) : base(client, baseUrl)
        {
        }

        public async Task<ContentModel[]> GetPagedContent()
        {
            var request = new RestRequest($"{BaseUrl}{UriHelper.GetPagedContent}", Method.Get);
            var response = await Client.ExecuteAsync<ContentModel[]>(request);
            return response.Data;
        }
    }
}
