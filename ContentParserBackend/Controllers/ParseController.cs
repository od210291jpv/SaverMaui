using ContentParserBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ContentParserBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParseController : Controller
    {
        private IRabbitMqService mqService;

        public ParseController(IRabbitMqService rabbit)
        {
            this.mqService = rabbit;
        }

        [HttpGet()]
        public HttpStatusCode Index(string keyword, int parserId = 1, byte? desiredRate = null)
        {
            if (parserId == 2)
            {
                this.mqService.SendMessage($"{keyword}:{parserId}", "ParcePediaContentQueue");
            }
            else 
            {
                var rate = desiredRate ?? 0;
                this.mqService.SendMessage($"{keyword}:{rate}", "ParceContentQueue");
            }

            return HttpStatusCode.OK;
        }
    }
}
