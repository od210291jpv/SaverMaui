using RestSharp;
using SaverBackendApiClient.DTO.Profile;
using System.Net;

namespace SaverBackendApiClient.ServiceActions
{
    public class UserProfileServiceActions : BaseServiceAction
    {
        public UserProfileServiceActions(RestClient client, string baseUrl) : base(client, baseUrl)
        {
        }

        public async Task<HttpStatusCode> RegisterProfileAsync(RegisterProfileRequestModel requestModel) 
        {
            RestRequest request = new RestRequest($"{this.BaseUrl}{Helpers.UriHelper.RegisterProfile}", Method.Post);
            request.AddJsonBody(requestModel);
            RestResponse response = await this.Client.ExecuteAsync(request);
            return response.StatusCode;
        }
    }
}
