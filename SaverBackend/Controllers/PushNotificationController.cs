using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using SaverBackend.DTO;
using SaverBackend.Hubs;
using SaverBackend.Models;
using WebLoggerClient;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PushNotificationController : Controller
    {
        private ApplicationContext db;
        private IHubContext<MainNotificationsHub> notificationsHubContext;
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public PushNotificationController(ApplicationContext database, IHubContext<MainNotificationsHub> hubcontext)
        {
            this.db = database;
            this.notificationsHubContext = hubcontext;
        }

        [HttpPost(Name = "PushNotification")]
        public async Task<IActionResult> Index(PushNotificationDto pushNotification)
        {
            await this.webLogger.LogAsync($"Received push notification request with message: {pushNotification.NotificationMessage}", LogSeverity.Verbose);
            await this.notificationsHubContext.Clients.All.SendAsync("SendNotificationsAsync", pushNotification.NotificationMessage);
            await this.webLogger.LogAsync("Push notification sent to all clients", LogSeverity.Verbose);
            return StatusCode(200);
        }
    }
}
