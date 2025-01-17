
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SaverBackend.Models;
using System.Text;

namespace SaverBackend.Services.RabbitMq
{
    public class ContentInfoListener : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly IRabbitMqService mqService;

        public ContentInfoListener(IRabbitMqService mqService, IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "InitContentQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.mqService = mqService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (message.ToLower().Trim() == "init")
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                    var content = context.Contents.Where(c => c.Cost == 0).ToArray();

                    foreach (var c in content)
                    {
                        c.Cost = new Random().Next(120, 1500);
                    }

                    await context.SaveChangesAsync();
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("InitContentQueue", false, consumer);

            return Task.CompletedTask;
        }
    }
}
