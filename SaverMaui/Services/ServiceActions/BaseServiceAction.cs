using RestSharp;

namespace SaverMaui.Services.ServiceActions
{
    public class BaseServiceAction
    {
        protected RestClient client = new();
    }
}
