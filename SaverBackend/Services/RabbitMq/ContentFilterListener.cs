
using ImageRecognitionNN;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using SaverBackend.DTO;
using StackExchange.Redis;
using System.Text;

namespace SaverBackend.Services.RabbitMq
{
    public class ContentFilterListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        public ContentFilterListener()
        {
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "FilterContentQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            var redisDb = redis.GetDatabase(2);

            RedisKey[] allKeys = redis.GetServer("192.168.88.252:6379").Keys(2).ToArray();
            string[] allContent = allKeys.AsParallel().Select(k => redisDb.StringGet(k).ToString()).ToArray();

            _ = allKeys.AsParallel().Select(k => redisDb.StringGetDelete(k).ToString()).ToArray();

            ImageAnalyzer nn = new ImageAnalyzer();
            RestClient client = new RestClient();

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var category = Encoding.UTF8.GetString(ea.Body.ToArray());

                //string[] filteredContent = allContent.AsParallel().Where(c => nn.AnalyzeImageByUrl(c).Result.PredictedLabel.ToLower() == category.ToLower()).ToArray();
                string[] filteredContent = Array.Empty<string>();

                foreach (var item in allContent) 
                {
                    var label = await nn.AnalyzeImageByUrl(item);
                    if (label.PredictedLabel.ToLower() == category.ToLower()) 
                    {
                        await redisDb.StringSetAsync(Guid.NewGuid().ToString(), item);

                        await client.ExecutePostAsync(new RestRequest("http://192.168.88.252/PushNotification", Method.Post).AddJsonBody<PushNotificationDto>(new PushNotificationDto()
                        {
                            NotificationMessage = $"Filtered countent found: {item}",
                        }));
                    }
                }

                await client.ExecutePostAsync(new RestRequest("http://192.168.88.252/PushNotification", Method.Post).AddJsonBody<PushNotificationDto>(new PushNotificationDto()
                {
                    NotificationMessage = $"Filtering done",
                }));

                _channel.BasicAck(ea.DeliveryTag, false);

                allKeys = Array.Empty<RedisKey>();
                allContent = Array.Empty<string>();
                filteredContent = Array.Empty<string>();
            };

            _channel.BasicConsume("FilterContentQueue", false, consumer);

            return Task.CompletedTask;
        }
    }
}
