using RestSharp;
using SaverMaui.Services.Contracts.Profile;
using SaverMaui.Services.Helpers;

namespace SaverMaui.Services.ServiceActions
{
    public class UserServiceActions : BaseServiceAction
    {
        public async Task<ProfileInfoResponseDto> GetProfileInfo(string login, string password) 
        {
            RestRequest request = new(UriHelper.ProfileInfo(login, password), Method.Get);
            var response = await this.client.ExecuteAsync(request);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ProfileInfoResponseDto>(response.Content);
            return result;
        }
    }
}
