using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;

namespace ContentParserBackend.Services
{
    public class NuTvListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private IServiceScopeFactory serviceScopeFactory;
        private readonly IRabbitMqService mqService;

        public NuTvListener(IServiceScopeFactory serviceScopeFactory, IRabbitMqService rabbit)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "ParceContentQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.mqService = rabbit;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            var redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");
            var redisSearchStateDb = redis.GetDatabase(6);
            var redisDb = redis.GetDatabase(2);
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();


            consumer.Received += async (ch, ea) =>
            {
                await redisSearchStateDb.StringSetAsync("SearchStatus", "Active");

                var inpm = Encoding.UTF8.GetString(ea.Body.ToArray());
                var keyword = inpm.Split(":").First();

                foreach (var cha in alpha) 
                {
                    NuTvSearchEngine parser = new NuTvSearchEngine("https://nudostar.tv/", cha.ToString());
                    var results = await parser.ParseAsync(keyword, mqService);

                    foreach (var item in results)
                    {
                        await redisDb.StringSetAsync(Guid.NewGuid().ToString(), item);
                    }
                }

                await redisSearchStateDb.StringSetAsync("SearchStatus", "Passive");
            };

            _channel.BasicConsume("ParceContentQueue", false, consumer);
        }
    }
}
