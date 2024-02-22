using Flurl.Http;
using LiveHost.DataBase;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using System.Text;
using System.Threading.Channels;

namespace LiveHost.Services
{
    public class ContentDownloadService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        private IConnection _connection;
        private IModel _channel;

        public ContentDownloadService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;

            var factory = new ConnectionFactory { HostName = "192.168.88.55", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "PullContent", durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            ///

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());


                using var scope = serviceScopeFactory.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                var directory = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory());

                var allCategories = context.Categories.Select(c => c.Name).ToList();
                var allCatsAsObj = context.Categories.ToArray();

                foreach (var category in allCategories)
                {
                    Directory.CreateDirectory(directory + "/data/" + category);
                }

                var allContent = context.Contents.ToArray();

                RestClient client = new RestClient();


                foreach (var con in allContent) 
                {
                    System.Console.WriteLine($"Downloading file: {con.Title}");

                    if (con.CategoryId != null) 
                    {
                        try
                        {
                            var dir = allCatsAsObj.FirstOrDefault(c => c.CategoryId == con?.CategoryId);


                            var targetDir = dir != null ? dir.Name : "Rest";



                            Console.WriteLine($"For category: {targetDir}");

                            try 
                            {
                                await con.ImageUri.DownloadFileAsync(Directory.GetCurrentDirectory() + "/data/" + targetDir);
                            }
                            catch (Flurl.Http.FlurlHttpException) 
                            {
                                continue;
                            }
                        }

                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    
                }


                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("PullContent", false, consumer);

            return Task.CompletedTask;
        }
    }
}
