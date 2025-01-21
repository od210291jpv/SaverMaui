using RestSharp;
using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Content;
using SaverMaui.Services.Helpers;

namespace SaverMaui.Services.ServiceActions
{
    public class CategoryServiceActions : BaseServiceAction
    {
        public async Task<CategoryDto[]> GetAllCategories() 
        {
            var t = await this.client.ExecuteAsync(new RestRequest(UriHelper.GetAllCategories, Method.Get));
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CategoryDto[]>(t.Content);
            return result;
        }

        public async Task<ContentDto[]> GetCategoryContent(Guid categoryId) 
        {
            var response = await this.client.ExecuteGetAsync<ContentDto[]>(new RestRequest(UriHelper.GetCategoryContent(categoryId)));
            return response.Data;
        }
    }
}
