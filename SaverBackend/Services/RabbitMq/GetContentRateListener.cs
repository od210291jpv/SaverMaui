
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RateRecognitionNN;
using StackExchange.Redis;
using System;
using System.Text;

namespace SaverBackend.Services.RabbitMq
{
    public class GetContentRateListener : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;
        private IRabbitMqService RabbitService;

        public GetContentRateListener(IServiceScopeFactory serviceScopeFactory, IRabbitMqService rabbit)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "GetRateQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase();
            this.RabbitService = rabbit;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            ImageRateAnalyzer nn = new ImageRateAnalyzer();

            var redisDb = redis.GetDatabase(2);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var rate = message.Split("*").First();
                var url = message.Split("*").Last();

                RabbitService.SendMessage($"Recognize content rate: url:{url}, expected rate: {rate}", "NotificationsQueue");

                var r = await nn.AnalyzeImageByUrl(url);

                if (r.PredictedLabel == rate) 
                {
                    await redisDb.StringSetAsync(Guid.NewGuid().ToString(), url);
                    RabbitService.SendMessage($"Found match for {url}, rate is: {r}", "NotificationsQueue");
                }
            };

            return Task.CompletedTask;
        }
    }
}
