using System.Net;

namespace SaverBackend.DataStructures
{
    public class ContentAvailabilityInfo
    {
        public string Url { get; set; } = string.Empty;

        public string ContentTitle { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public HttpStatusCode StatusCode { get; set; }
    }
}
