using Newtonsoft.Json;

using SaverMaui.Services.Contracts.Profile;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.Interfaces;

using System.Net;

namespace SaverMaui.Services.ServiceExtensions
{
    internal static class UserProfileExtensions
    {
        public static async Task<HttpStatusCode> RegisterUser(this IHttpServiceClient serviceClient, string login, string password, string verificationCode = "string") 
        {
            var result = await serviceClient.PostRequestAsync(
                UriHelper.RegisterUser,
                new CreateProfileRequestDto() 
                { 
                    Password = password,
                    UserName = login,
                    VerificationCode = verificationCode 
                });

            return result.StatusCode;
        }

        public static async Task<LoginResponse> LoginUserAsync(this IHttpServiceClient serviceClient, string login, string password) 
        {
            var response = await serviceClient.PostRequestAsync(UriHelper.Login(login, password));
            return JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<HttpStatusCode> LogoutUserAsync(this IHttpServiceClient serviceClient) 
        {
            var response = await serviceClient.PostRequestAsync(UriHelper.Logout(Environment.Login, Environment.Password));
            return response.StatusCode;
        }

        public static async Task<bool> IsUserLoggedInAsync(this IHttpServiceClient serviceClient, string login, string password) 
        {
            var response = await serviceClient.GetRequestAsync(UriHelper.GetLoginStatus(login, password));

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
