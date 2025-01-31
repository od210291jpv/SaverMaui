using RestSharp;

using SaverMaui.Services.Contracts.Content;
using SaverMaui.Services.Helpers;

using System.Net;

namespace SaverMaui.Services.ServiceActions
{
    public class ContentServiceActions : BaseServiceAction
    {
        public async Task<ContentDto[]> GetAllContentAsync() 
        {
            RestRequest request = new RestRequest(UriHelper.GetAllContent, Method.Get);

            var result = await this.client.ExecuteGetAsync<ContentDto[]>(request);
            if (result.StatusCode == HttpStatusCode.OK) 
            {
                return result.Data;
            }

            return Array.Empty<ContentDto>();
        }

        public async Task<ContentDto[]> GetRatedContent(short rate = 0) 
        {
            var response = await this.client.ExecuteAsync(new(UriHelper.GetRatedContent(rate), Method.Get));

            if (response.StatusCode == HttpStatusCode.OK) 
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ContentDto[]>(response.Content);
                return result;
            }
            return Array.Empty<ContentDto>();
        }

        public async Task<ContentDto[]> GetAllContentWithPaginationAsync(short page, short size) 
        {
            RestRequest request = new RestRequest(UriHelper.GetPaginatedContent(page, size));

            var result = await this.client.ExecuteGetAsync<ContentDto[]>(request);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return result.Data;
            }

            return Array.Empty<ContentDto>();
        }

        public async Task<HttpStatusCode> DeleteContentAsync(int contentId) 
        {
            RestRequest request = new RestRequest(UriHelper.DeleteContent(contentId), Method.Delete);
            var response = await this.client.ExecuteAsync(request);

            return response.StatusCode;
        }

        public async Task<string[]> GetSearchResults() 
        {
            RestRequest request = new RestRequest(UriHelper.SearchResults, Method.Get);
            var response = await this.client.ExecuteGetAsync<string[]>(request);

            if (response.Data != null) 
            {
                return response.Data;
            }

            return Array.Empty<string>();
        }

        public async Task<RestResponse> SearchContent(string keyword) 
        {
            var response = await this.client.ExecuteGetAsync(new RestRequest(UriHelper.SearchContent(keyword), Method.Get));
            return response;
        }

        public async Task<RestResponse> DeleteSearchResults() 
        {
            var response = await this.client.ExecuteAsync(new RestRequest(UriHelper.CleanSearchResults, Method.Delete));
            return response;
        }

        public async Task<ContentDto> RateContent(int contentId, int profileId, short rate) 
        {
            var response = await this.client.ExecuteGetAsync<ContentDto>(new RestRequest(UriHelper.RateContent(contentId, rate, profileId), Method.Get));
            return response.Data;
        }

        public async Task<int[]> GetFavoriteContent(string login, string password) 
        {
            RestRequest request = new(UriHelper.GetFavoriteContent(login, password), Method.Post);
            request.AddJsonBody(new GetFavoriteContentRequestDto() { Logn = login, Password = password });

            var response = await this.client.ExecuteAsync<int[]>(request);
            return response.Data;
        }

        public async Task<ContentDto> GetContentById() 
        { }
    }
}
