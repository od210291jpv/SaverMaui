using Microsoft.AspNetCore.Mvc;
using SaverBackend.DataStructures;
using SaverBackend.Models;
using SaverBackend.Services.RabbitMq;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("Controller")]
    public class NotifyNewCardsAddedController : ControllerBase
    {
        private readonly ApplicationContext context;
        private readonly IRabbitMqService mqService;

        public NotifyNewCardsAddedController(IRabbitMqService rabbit, ApplicationContext context)
        {
            this.mqService = rabbit;
            this.context = context;
        }

        [HttpPost(Name = "NotifyNewCardsAdded")]
        public IActionResult Index(RabbitMqMessage message)
        {
            mqService.SendMessage(message, "CardsUpdateQueque");

            return Ok($"Message sent: {message.Topic}");
        }
    }
}
