using Microsoft.AspNetCore.SignalR;

namespace SaverBackend.Hubs
{
    public class MainNotificationsHub : Hub
    {
        public async Task SendNotificationsAsync(string message)
        {
            await this.Clients.All.SendAsync("SendNotificationsAsync", message);
        }
    }
}
