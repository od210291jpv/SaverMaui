using Microsoft.AspNetCore.Mvc;
using SaverBackend.DataStructures;
using SaverBackend.Models;
using SaverBackend.Services.RabbitMq;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnavailableContentController : ControllerBase
    {
        private readonly ApplicationContext context;
        private readonly IRabbitMqService mqService;

        public UnavailableContentController(ApplicationContext context, IRabbitMqService mqService)
        {
            this.context = context;
            this.mqService = mqService;
        }

        [HttpPost(Name = "GetUnavailableContent")]
        public IActionResult Index(RabbitMqMessage message)
        {
            mqService.SendMessage(message);

            return Ok($"Message sent: {message.Topic}");
        }
    }
}
