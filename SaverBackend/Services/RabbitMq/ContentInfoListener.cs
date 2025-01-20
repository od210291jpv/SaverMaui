
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SaverBackend.Models;
using StackExchange.Redis;
using System.Text;

namespace SaverBackend.Services.RabbitMq
{
    public class ContentInfoListener : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;
        private IDatabase redisContentDb;

        public ContentInfoListener(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "InitContentQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase();
            this.redisContentDb = redis.GetDatabase(1);
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
                        var cost = new Random().Next(120, 1500);
                        c.Cost = cost;
                        var redisEntry = await this.redisContentDb.StringGetAsync(c.Id.ToString());

                        if (redisEntry.HasValue == true) 
                        {
                            var redisContent = JsonConvert.DeserializeObject<Content>(redisEntry);
                            redisContent.Cost = cost;

                            await this.redisContentDb.StringSetAsync(c.Id.ToString(), JsonConvert.SerializeObject(redisContent));
                        }
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
