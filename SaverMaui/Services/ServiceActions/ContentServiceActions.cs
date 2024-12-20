﻿using RestSharp;

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
    }
}
