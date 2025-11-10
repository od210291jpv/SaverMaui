
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using SaverBackend.DTO;
using SaverBackend.Hubs;
using System.Text;

namespace SaverBackend.Services.RabbitMq
{
    public class NotificationsListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private IHubContext<MainNotificationsHub> notificationsHubContext;
       // private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public NotificationsListener(IHubContext<MainNotificationsHub> hubcontext)
        {
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "NotificationsQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.notificationsHubContext = hubcontext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            //await Task.Run(async () => await this.webLogger.LogAsync("NotificationsListener service started", LogSeverity.Verbose));
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
              //  await this.webLogger.LogAsync("Message received in NotificationsListener, processing push notification", LogSeverity.Verbose);
                var pushMessage = Encoding.UTF8.GetString(ea.Body.ToArray());

             //   await this.webLogger.LogAsync($"Received push notification message: {pushMessage}", LogSeverity.Verbose);
                RestClient client = new RestClient();

            //    await this.webLogger.LogAsync("Sending push notification to PushNotificationController", LogSeverity.Verbose);
                await client.ExecutePostAsync(new RestRequest("http://192.168.88.252/PushNotification", Method.Post).AddJsonBody<PushNotificationDto>(new PushNotificationDto() 
                {
                    NotificationMessage = pushMessage,
                }));

                //await this.webLogger.LogAsync("Push notification sent successfully", LogSeverity.Verbose);

                _channel.BasicAck(ea.DeliveryTag, false);
                //await this.webLogger.LogAsync("Message acknowledged in NotificationsListener", LogSeverity.Verbose);
            };

            //await Task.Run(async () => await this.webLogger.LogAsync("Starting to consume messages from NotificationsQueue", LogSeverity.Verbose));
            _channel.BasicConsume("NotificationsQueue", false, consumer);
            //await Task.Run(async () => await this.webLogger.LogAsync("NotificationsListener is now listening for messages", LogSeverity.Verbose));
            //return Task.CompletedTask;
        }
    }
}
