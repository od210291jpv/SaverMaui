using LiveHost.DataStructures;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using SaverBackend.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace LiveHost.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public RabbitMqListener(IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "MyQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                RabbitMqMessage? deserializedContent = JsonSerializer.Deserialize<RabbitMqMessage>(content);
                this.ParseUnavailableContent();

                Debug.WriteLine($"Получено сообщение: {content}");

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("MyQueue", false, consumer);

            return Task.CompletedTask;
        }

        public async void ParseUnavailableContent() 
        {
            List<ContentAvailabilityInfo> unavailableContent = new List<ContentAvailabilityInfo>();

            using var scope = serviceScopeFactory.CreateScope();
            
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            Content[] allContent = await context.Contents.ToArrayAsync();

                // now do your work
            

            RestClient client = new RestClient();

            foreach (var content in allContent)
            {
                if (content.ImageUri != null && content.ImageUri != "")
                {
                    var result = await client.ExecuteGetAsync(new RestRequest(content.ImageUri, Method.Get));

                    if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (!context.BrokenContents.Select(c => c.Id).Contains(content.Id)) 
                        {
                            context.BrokenContents.Add(new BrokenContent()
                            {
                                CategoryId = content.CategoryId,
                                Id = content.Id,
                                ImageUri = content.ImageUri,
                                Title = content.Title,
                            });

                            await context.SaveChangesAsync();

                            System.Console.WriteLine($"Orphan content found, {content.Title}");
                        }
                    }
                }
            }

        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
