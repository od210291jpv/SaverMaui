namespace SaverMaui.Services.Interfaces
{
    public interface IHttpServiceClient
    {
        Task<HttpResponseMessage> GetRequestAsync(string url);

        Task<HttpResponseMessage> PostRequestAsync(string endpoint, IRequest request);

        Task<HttpResponseMessage> PostRequestAsync(string endpoint);
    }
}
