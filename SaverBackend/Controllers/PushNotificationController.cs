using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SaverBackend.DTO;
using SaverBackend.Hubs;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PushNotificationController : Controller
    {
        private ApplicationContext db;
        private IHubContext<MainNotificationsHub> notificationsHubContext;

        public PushNotificationController(ApplicationContext database, IHubContext<MainNotificationsHub> hubcontext)
        {
            this.db = database;
            this.notificationsHubContext = hubcontext;
        }

        [HttpPost(Name = "PushNotification")]
        public async Task<IActionResult> Index(PushNotificationDto pushNotification)
        {
            await this.notificationsHubContext.Clients.All.SendAsync("SendNotificationsAsync", pushNotification.NotificationMessage);
            
            return StatusCode(200);
        }
    }
}
