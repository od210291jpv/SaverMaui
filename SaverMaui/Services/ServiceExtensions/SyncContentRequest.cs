using Newtonsoft.Json;
using SaverMaui.Services.Contracts;
using SaverMaui.Services.Contracts.Video;
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

        public async static Task<int[]> AddVideoRequest(this IHttpServiceClient serviceClient, SyncVideoRequestDto requestModel) 
        {
            var response = await serviceClient.PostRequestAsync(UriHelper.AddVideo, requestModel);
            return JsonConvert.DeserializeObject<int[]>(await response.Content.ReadAsStringAsync());
        }

        public async static Task<AddVideoRequestDto[]> GetVideoRequest(this IHttpServiceClient serviceClient, Guid profileId)
        {
            var response = await serviceClient.GetRequestAsync(UriHelper.GetUserVideo(profileId));
            return JsonConvert.DeserializeObject<AddVideoRequestDto[]>(await response.Content.ReadAsStringAsync());
        }
    }
}
