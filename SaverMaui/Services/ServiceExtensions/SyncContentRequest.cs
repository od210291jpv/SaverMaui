using Newtonsoft.Json;
using SaverMaui.Services.Contracts;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.Interfaces;
using System.Net;

namespace SaverMaui.Services.ServiceExtensions
{
    public static class SyncContentRequest
    {
        public static async Task<HttpStatusCode> PostAllContentDataAsync(this IHttpServiceClient serviceClient, PostContentDataRequest requestData)
        {
            var result = await serviceClient.PostRequestAsync(UriHelper.PostContent, requestData);

            return result.StatusCode;
        }

        public async static Task<GetAllCategoriesResponseModel[]> GetAllCategoriesAsync(this BackendServiceClient serviceClient) 
        {
            var response = await serviceClient.GetRequestAsync(UriHelper.GetAllCategories);
            var st = response.StatusCode;

            return JsonConvert.DeserializeObject<GetAllCategoriesResponseModel[]>(await response.Content.ReadAsStringAsync());

        }

        public async static Task<GetAllContentResponseModel[]> GetAllContentAsync(this IHttpServiceClient serviceClient) 
        {
            var response = await serviceClient.GetRequestAsync(UriHelper.GetAllContent);
            return JsonConvert.DeserializeObject<GetAllContentResponseModel[]>(await response.Content.ReadAsStringAsync());
        }
    }
}
