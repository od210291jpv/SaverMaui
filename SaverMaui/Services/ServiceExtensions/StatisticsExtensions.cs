using Newtonsoft.Json;

using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Statistics;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.Interfaces;
using System.Threading.Tasks;

namespace SaverMaui.Services.ServiceExtensions
{
    public static class StatisticsExtensions
    {
        public static async Task<CategoryDto> UpdateCategoryStatisticsAsync(this IHttpServiceClient serviceClient, Guid categoryId, int amountOfOpenings, int amountOfFavorites) 
        {
            var result =  await serviceClient
                .PostRequestAsync(UriHelper.UpdateStatistics,
                new UpdateStatisticsDto() 
                { 
                    CategoryId = categoryId,
                    Favorites = amountOfFavorites,
                    Openings = amountOfOpenings 
                });

            return JsonConvert.DeserializeObject<CategoryDto>(await result.Content.ReadAsStringAsync());
        }

        public static async Task<StatisticsDto> GetCategoriesStatisticsAsync(this IHttpServiceClient serviceClient) 
        {
            HttpResponseMessage response = await serviceClient.GetRequestAsync(UriHelper.CategoriesStatistics);

            return JsonConvert.DeserializeObject<StatisticsDto>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<CategoryDto[]> GetMostPopularCategories(this IHttpServiceClient serviceClient, int limit = 10) 
        {
            var response = await serviceClient.PostRequestAsync(UriHelper.GetMostPopularCategories(limit));

            return JsonConvert.DeserializeObject<CategoryDto[]>(await response.Content.ReadAsStringAsync());
        }
    }
}
