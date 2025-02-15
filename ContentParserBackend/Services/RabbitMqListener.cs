
using HtmlAgilityPack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using StackExchange.Redis;
using System.Data;
using System.Text;
using System.Web;
using static System.Net.WebRequestMethods;

namespace ContentParserBackend.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private string keyword = string.Empty;
        private readonly IRabbitMqService mqService;

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

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            double progress = 0;
            double step = 0.03846153846;

            consumer.Received += async (ch, ea) =>
            {
                var inpm = Encoding.UTF8.GetString(ea.Body.ToArray());
                var keyword = inpm.Split(":").First();
                var rate = inpm.Split(":").Last();

                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();
                
                foreach (char c in alpha)
                {
                    mqService.SendMessage($"Parsing {c} character for request {keyword} https://fapomania.com/onlyfans/{c} link", "NotificationsQueue");
                    SerachEngine parser = new($"https://fapomania.com/onlyfans/{c}/");
                    var result = await parser.ParseAsync(keyword);

                    var resultLinks = PullImagesLinks(result, keyword);

                    if (resultLinks.Count() > 0) 
                    {
                        mqService.SendMessage($"For request {keyword} https://fapomania.com/onlyfans/{c} found {resultLinks.Count()} results", "NotificationsQueue");
                    }

                    var redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
                    var redisDb = redis.GetDatabase(2);

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
                }

                progress = 0;

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("ParceContentQueue", false, consumer);

            return Task.CompletedTask;
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
