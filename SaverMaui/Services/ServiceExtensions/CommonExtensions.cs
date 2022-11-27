using Newtonsoft.Json;

using SaverMaui.Services.Helpers;
using SaverMaui.Services.Interfaces;

namespace SaverMaui.Services.ServiceExtensions
{
    public static class CommonExtensions
    {
        public async static Task<bool> PingAsync(this IHttpServiceClient serviceClient) 
        {
            var response = await serviceClient.GetRequestAsync(UriHelper.Ping);

            return JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
        }
    }
}
