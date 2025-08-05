using RestSharp;

using SaverMaui.Services.Contracts.Profile;
using SaverMaui.Services.Helpers;

using System.Net;

namespace SaverMaui.Services.ServiceActions
{
    public class UserServiceActions : BaseServiceAction
    {
        public async Task<ProfileInfoResponseDto> GetProfileInfo(string login, string password) 
        {
            RestRequest request = new(UriHelper.ProfileInfo(login, password), Method.Get);
            var response = await this.client.ExecuteAsync(request);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                return new ProfileInfoResponseDto();
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ProfileInfoResponseDto>(response.Content);
            return result;
        }
    }
}
