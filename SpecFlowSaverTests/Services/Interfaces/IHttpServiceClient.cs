using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlowSaverTests.Services.Interfaces
{
    public interface IHttpServiceClient
    {
        Task<HttpResponseMessage> GetRequestAsync(string url);

        Task<HttpResponseMessage> PostRequestAsync(string endpoint, IRequest request);
    }
}
