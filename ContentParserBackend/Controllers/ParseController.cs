using ContentParserBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using WebLoggerClient;

namespace ContentParserBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParseController : Controller
    {
        private IRabbitMqService mqService;
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public ParseController(IRabbitMqService rabbit)
        {
            this.mqService = rabbit;
        }

        [HttpGet()]
        public async Task<HttpStatusCode> Index(string keyword, int parserId = 1, byte? desiredRate = null)
        {
            await this.webLogger.LogAsync($"Received parsing request with keyword: {keyword}, parserId: {parserId}, desiredRate: {desiredRate}", LogSeverity.Verbose);
            if (parserId == 2)
            {
                await this.webLogger.LogAsync("Using specialized parser with parserId 2", LogSeverity.Verbose);
                this.mqService.SendMessage($"{keyword}:{parserId}", "ParcePediaContentQueue");
                await this.webLogger.LogAsync("Message sent to ParcePediaContentQueue", LogSeverity.Verbose);
            }
            else 
            {
                await this.webLogger.LogAsync("Using default parser", LogSeverity.Verbose);
                var rate = desiredRate ?? 0;
                this.mqService.SendMessage($"{keyword}:{rate}", "ParceContentQueue");
                await this.webLogger.LogAsync("Message sent to ParceContentQueue", LogSeverity.Verbose);
            }

            await this.webLogger.LogAsync("Parsing request processing completed", LogSeverity.Verbose);
            return HttpStatusCode.OK;
        }
    }
}
