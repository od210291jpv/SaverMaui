
using HtmlAgilityPack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using StackExchange.Redis;
using System.Data;
using System.Text;
using WebLoggerClient;

namespace ContentParserBackend.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private string keyword = string.Empty;
        private readonly IRabbitMqService mqService;
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public RabbitMqListener(IServiceScopeFactory serviceScopeFactory, IRabbitMqService rabbit)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "ParceContentQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: "GetRateQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.mqService = rabbit;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () => await this.webLogger.LogAsync("RabbitMqListener service started", LogSeverity.Verbose));
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            await Task.Run(async () => await this.webLogger.LogAsync("Connected to RabbitMQ and waiting for messages in ParceContentQueue", LogSeverity.Verbose));
            double progress = 0;
            await Task.Run(async () => await this.webLogger.LogAsync("Initialized progress tracking for content parsing", LogSeverity.Verbose));
            double step = 0.03846153846;

            var redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");
            var redisSearchStateDb = redis.GetDatabase(6);
            var redisDb = redis.GetDatabase(2);
            await Task.Run(async () => await this.webLogger.LogAsync("Connected to Redis database 6 for search state tracking", LogSeverity.Verbose));

            consumer.Received += async (ch, ea) =>
            {
                await this.webLogger.LogAsync("Message received in RabbitMqListener, starting content parsing process", LogSeverity.Verbose);
                await redisSearchStateDb.StringSetAsync("SearchStatus", "Active");
                await this.webLogger.LogAsync("Set SearchStatus to Active in Redis database 6", LogSeverity.Verbose);

                var inpm = Encoding.UTF8.GetString(ea.Body.ToArray());
                var keyword = inpm.Split(":").First();
                var rate = inpm.Split(":").Last();

                await this.webLogger.LogAsync($"Received parsing request for keyword: {keyword} with rate: {rate}", LogSeverity.Verbose);
                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();

                // fix, get from config
                

                foreach (char c in alpha)
                {
                    mqService.SendMessage($"Parsing {c} character for request {keyword} https://fapomania.com/onlyfans/{c} link", "NotificationsQueue");
                    await this.webLogger.LogAsync($"Starting parsing for character: {c}", LogSeverity.Verbose);
                    SerachEngine parser = new($"https://fapomania.com/onlyfans/{c}/");
                    var result = await parser.ParseAsync(keyword, mqService);

                    var resultLinks = PullImagesLinks(result, keyword);

                    if (resultLinks.Count() > 0) 
                    {
                        await this.webLogger.LogAsync($"Found {resultLinks.Count()} results for request {keyword} https://fapomania.com/onlyfans/{c}", LogSeverity.Verbose);
                        mqService.SendMessage($"For request {keyword} https://fapomania.com/onlyfans/{c} found {resultLinks.Count()} results", "NotificationsQueue");
                    }

                    foreach (var rl in resultLinks)
                    {
                        if (rate != "0")
                        {
                            mqService.SendMessage($"{rate}*{rl}", "GetRateQueue");
                        }
                        else 
                        {
                            await redisDb.StringSetAsync(Guid.NewGuid().ToString(), rl);
                        }
                    }

                    progress += step;
                    mqService.SendMessage($"Search Progress:{progress}", "NotificationsQueue");
                    mqService.SendMessage($"Found p results:{resultLinks.Count()}", "NotificationsQueue");
                    await this.webLogger.LogAsync($"Completed parsing for character: {c}, updated progress to {progress}", LogSeverity.Verbose);
                }

                progress = 0;

                await this.webLogger.LogAsync($"Completed parsing for keyword: {keyword}, setting SearchStatus to Passive", LogSeverity.Verbose);
                _channel.BasicAck(ea.DeliveryTag, false);
                await redisSearchStateDb.StringSetAsync("SearchStatus", "Passive");
            };

            _channel.BasicConsume("ParceContentQueue", false, consumer);
            await Task.Run(async () => await this.webLogger.LogAsync("RabbitMqListener is now listening for messages in ParceContentQueue", LogSeverity.Verbose));

            //return Task.CompletedTask;
        }



        public static List<string> PullImagesLinks(List<string> urls, string keyword)
        {
            RestClient client = new RestClient();

            List<string> links = new List<string>();

            foreach (string url in urls)
            {
                HtmlDocument doc = new HtmlDocument();
                var dt = client.ExecuteGet<string>(new RestRequest(url, Method.Get)).Content;
                doc.LoadHtml(dt);

                var imgLinks = doc.DocumentNode.SelectNodes("//div[@class = 'previzakoimag']/img")?.Select(i => i.GetAttributeValue("src", "")).Where(e => e != "").Select(i => i.Replace("_300px", "")).ToList();
                if (imgLinks is not null) 
                {
                    links.AddRange(imgLinks.Where(l => l != null && l.ToLower().Contains(keyword.ToLower()) == true));
                }
            }

            return links;
        }
    }
}
