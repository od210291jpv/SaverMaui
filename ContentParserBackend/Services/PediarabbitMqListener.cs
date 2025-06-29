
using HtmlAgilityPack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using StackExchange.Redis;
using System.Text;

namespace ContentParserBackend.Services
{
    public class PediarabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IRabbitMqService mqService;

        public PediarabbitMqListener(IRabbitMqService rabbit)
        {
            this.mqService = rabbit;
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "ParcePediaContentQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var keyword = Encoding.UTF8.GetString(ea.Body.ToArray());

                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

                var parserId = keyword.Split(":")?[1];
                var expectedKeyword = keyword.Split(":")?[0] ?? keyword;

                var redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
                var redisDb = redis.GetDatabase(2);

                foreach (char c in alpha)
                {
                    mqService.SendMessage($"Parsing {c} character for request {keyword} https://fapopedia.net/list/{c} link", "NotificationsQueue");

                    var result = await new PediaSearchEngine($"https://fapopedia.net/list/{c}/").ParseAsync(expectedKeyword, mqService);
                    
                    mqService.SendMessage($"Pedia: {result.Count} mathed links found", "NotificationsQueue");

                    var parsedRsults = await this.PullPediaImageLinks(result, expectedKeyword);

                    foreach (var rl in parsedRsults)
                    {
                        mqService.SendMessage($"Pedia: {rl} found for {expectedKeyword}", "NotificationsQueue");
                        await redisDb.StringSetAsync(Guid.NewGuid().ToString(), rl);
                    }
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("ParcePediaContentQueue", false, consumer);

            return Task.CompletedTask;
        }

        public async Task<string[]> PullPediaImageLinks(List<string> urls, string keyword)
        {
            RestClient client = new RestClient();

            List<string> links = new List<string>();

            string xpath = "//div[@class = 'shrt-pc']/img";

            foreach (string url in urls)
            {
                HtmlDocument doc = new HtmlDocument();
                var dt = await client.ExecuteGetAsync<string>(new RestRequest(url, Method.Get));
                doc.LoadHtml(dt.Content ?? "");

                var link = doc.DocumentNode.SelectNodes(xpath)?.AsParallel().Select(i => i.GetAttributeValue("src", "")).Where(l => l != "" && l != null).Select(l => l.Replace(@"/t_", "")).ToArray();
                mqService.SendMessage($"Pedia: {link} was pulled for {keyword} in {url}", "NotificationsQueue");

                if (link != null)
                {
                    foreach (var l in link)
                    {
                        mqService.SendMessage($"Pedia: {l} was pulled for {keyword} in {url}", "NotificationsQueue");
                        if (l.ToLower().Contains(keyword.ToLower()))
                        {
                            mqService.SendMessage($"Pedia: {l} does contain keyword {keyword} in {url}", "NotificationsQueue");
                            links.Add(l);
                        }
                    }
                }
            }

            return links.Distinct().ToArray();
        }
    }
}
