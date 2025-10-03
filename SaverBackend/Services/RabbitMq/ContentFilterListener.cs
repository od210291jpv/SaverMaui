
using ImageRecognitionNN;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
using WebLoggerClient;

namespace SaverBackend.Services.RabbitMq
{
    public class ContentFilterListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IRabbitMqService mqService;
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public ContentFilterListener(IRabbitMqService mqService)
        {
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "FilterContentQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.mqService = mqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () => await this.webLogger.LogAsync("ContentFilterListener service started", LogSeverity.Verbose));
            var redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            var redisDb = redis.GetDatabase(2);

            Task.Run(async () => await this.webLogger.LogAsync("Connected to Redis database 2 for content filtering", LogSeverity.Verbose));
            RedisKey[] allKeys = redis.GetServer("192.168.88.252:6379").Keys(2).ToArray();
            string[] allContent = allKeys.AsParallel().Select(k => redisDb.StringGet(k).ToString()).ToArray();

            this.webLogger.LogAsync($"Total content items to filter: {allContent.Length}", LogSeverity.Verbose);
            _ = allKeys.AsParallel().Select(k => redisDb.StringGetDelete(k).ToString()).ToArray();

            this.webLogger.LogAsync("Cleared Redis database 2 after fetching content for filtering", LogSeverity.Verbose);
            
            this.webLogger.LogAsync("Initializing ImageAnalyzer for content filtering", LogSeverity.Verbose);
            ImageAnalyzer nn = new ImageAnalyzer();

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                this.webLogger.LogAsync("Message received in ContentFilterListener, starting content filtering process", LogSeverity.Verbose);
                var category = Encoding.UTF8.GetString(ea.Body.ToArray());

                this.webLogger.LogAsync($"Filtering content for category: {category}", LogSeverity.Verbose);
                string[] filteredContent = Array.Empty<string>();
                int counter = 0;

                this.webLogger.LogAsync("Starting content analysis using ImageAnalyzer", LogSeverity.Verbose);
                foreach (var item in allContent) 
                {
                    var label = await nn.AnalyzeImageByUrl(item);
                    if (label.PredictedLabel.ToLower() == category.ToLower()) 
                    {
                        await redisDb.StringSetAsync(Guid.NewGuid().ToString(), item);
                        counter++;
                        mqService.SendMessage($"Item found:{item}", "NotificationsQueue");
                        mqService.SendMessage($"Items processed:{counter}/{allContent.Length}", "NotificationsQueue");
                    }
                }
                this.webLogger.LogAsync($"Content filtering completed. Total items matched for category '{category}': {counter}", LogSeverity.Verbose);

                _channel.BasicAck(ea.DeliveryTag, false);
                this.webLogger.LogAsync("Message acknowledged in ContentFilterListener", LogSeverity.Verbose);
                allKeys = Array.Empty<RedisKey>();
                allContent = Array.Empty<string>();
                filteredContent = Array.Empty<string>();
                this.webLogger.LogAsync("Cleared temporary data in ContentFilterListener", LogSeverity.Verbose);
            };

            _channel.BasicConsume("FilterContentQueue", false, consumer);

            //return Task.CompletedTask;
        }
    }
}
