
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

        public NotificationsListener(IHubContext<MainNotificationsHub> hubcontext)
        {
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "NotificationsQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.notificationsHubContext = hubcontext;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var pushMessage = Encoding.UTF8.GetString(ea.Body.ToArray());

                RestClient client = new RestClient();
                await client.ExecutePostAsync(new RestRequest("http://192.168.88.252/PushNotification", Method.Post).AddJsonBody<PushNotificationDto>(new PushNotificationDto() 
                {
                    NotificationMessage = pushMessage,
                }));

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("NotificationsQueue", false, consumer);

            return Task.CompletedTask;
        }
    }
}
