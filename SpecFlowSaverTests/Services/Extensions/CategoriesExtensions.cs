using Newtonsoft.Json;

using SpecFlowSaverTests.Support.Models;
using System.Net;

namespace SpecFlowSaverTests.Services.Extensions
{
    internal static class CategoriesExtensions
    {
        internal async static Task<AllCategoriesResponseModel[]> GetAllCategoriesAsync(this BackendServiceClient serviceClient)
        {
            var response = await serviceClient.GetRequestAsync(UriHelper.GetAllCategories);
            return JsonConvert.DeserializeObject<AllCategoriesResponseModel[]>(await response.Content.ReadAsStringAsync());
        }

        internal async static Task<HttpStatusCode> CreateCategoryAsync(this BackendServiceClient serviceClient, CreateCategoryRequestModel requestModel) 
        {
            var response = await serviceClient.PostRequestAsync(UriHelper.CreateCategory, requestModel);
            return response.StatusCode;
        }

        internal static async Task<HttpStatusCode> DeleteCategoryAsync(this BackendServiceClient serviceClient, Guid categoryId)
        {
            var response = await serviceClient.PostRequestAsync(UriHelper.DeleteCategory(categoryId));
            return response.StatusCode;
        }
    }
}
