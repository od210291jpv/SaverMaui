using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace SaverMaui.Services.ServiceActions
{
    public class BaseServiceAction
    {
        protected RestClient client = new(
    configureSerialization: s => s.UseNewtonsoftJson());
    }
}
