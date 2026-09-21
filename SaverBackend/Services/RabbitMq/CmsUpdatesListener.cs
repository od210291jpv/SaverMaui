using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using SaverBackend.DTO.Events.CMS;

using SaverBackend.Models;

using StackExchange.Redis;

using System.Text;

namespace SaverBackend.Services.RabbitMq
{
    public class CmsUpdatesListener : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;
        private IRabbitMqService RabbitService;
        private IServiceScopeFactory scopeFactory;

        public CmsUpdatesListener(IServiceScopeFactory serviceScopeFactory, IRabbitMqService rabbit, IServiceScopeFactory scopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "CMSupdates", durable: true, exclusive: false, autoDelete: false, arguments: null);
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase();
            this.RabbitService = rabbit;
            this.scopeFactory = scopeFactory;           
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, msg) => 
            {
                CmsContentEvent? deserialized = JsonConvert.DeserializeObject<CmsContentEvent>(Encoding.UTF8.GetString(msg.Body.ToArray()));
                Console.WriteLine(deserialized);

                if (deserialized is not null) 
                {
                    switch (deserialized.EventType)
                    {
                        case Constants.Enums.ContentEventType.Created:
                            break;
                        case Constants.Enums.ContentEventType.Deleted:
                            using (var scope = scopeFactory.CreateScope())
                            {
                                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                                var contentToDelete = context.Contents.FirstOrDefault(c => c.CmsExternalId == deserialized.Id);

                                if (contentToDelete is not null) 
                                {
                                    contentToDelete.IsDeleted = deserialized.IsDeleted;
                                    await context.SaveChangesAsync();
                                }

                                await context.SaveChangesAsync();
                                await this.ResyncRedis();

                            }
                            break;
                        case Constants.Enums.ContentEventType.Edited:
                            using (var scope = scopeFactory.CreateScope()) 
                            {
                                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                                Content? contentToUpdate = context.Contents.FirstOrDefault(c => c.CmsExternalId == deserialized.Id);

                                if (contentToUpdate is not null) 
                                {
                                    contentToUpdate.IsDeleted = deserialized.IsDeleted;
                                    contentToUpdate.IsPublic = deserialized.IsPublic;
                                    contentToUpdate.ImageUri = deserialized.Path;
                                    contentToUpdate.IsEnabled = deserialized.Enabled;
                                    contentToUpdate.Title = deserialized.Description ?? contentToUpdate.Title;
                                    await context.SaveChangesAsync();
                                    await this.ResyncRedis();

                                }
                            }
                            break;
                        case Constants.Enums.ContentEventType.Enabled:
                            using (var scope = scopeFactory.CreateScope())
                            {
                                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                                Content? contentToUpdate = context.Contents.FirstOrDefault(c => c.CmsExternalId == deserialized.Id);

                                if (contentToUpdate is not null)
                                {
                                    contentToUpdate.IsEnabled = deserialized.Enabled;
                                    await context.SaveChangesAsync();
                                    await this.ResyncRedis();

                                }
                            }
                            break;
                        case Constants.Enums.ContentEventType.Disabled:
                            using (var scope = scopeFactory.CreateScope())
                            {
                                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                                Content? contentToUpdate = context.Contents.FirstOrDefault(c => c.CmsExternalId == deserialized.Id);
                                if (contentToUpdate is not null)
                                {
                                    contentToUpdate.IsEnabled = deserialized.Enabled;
                                    await context.SaveChangesAsync();
                                    await this.ResyncRedis();

                                }
                            }
                            break;
                        case Constants.Enums.ContentEventType.MadePrivate:
                            using (var scope = scopeFactory.CreateScope())
                            {
                                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                                Content? contentToUpdate = context.Contents.FirstOrDefault(c => c.CmsExternalId == deserialized.Id);
                                if (contentToUpdate is not null)
                                {
                                    contentToUpdate.IsPublic = deserialized.IsPublic;
                                    await context.SaveChangesAsync();
                                    await this.ResyncRedis();

                                }
                            }
                            break;
                        case Constants.Enums.ContentEventType.MadePublic:
                            using (var scope = scopeFactory.CreateScope())
                            {
                                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                                Content? contentToUpdate = context.Contents.FirstOrDefault(c => c.CmsExternalId == deserialized.Id);
                                if (contentToUpdate is not null)
                                {
                                    contentToUpdate.IsPublic = deserialized.IsPublic;
                                    await context.SaveChangesAsync();
                                    await this.ResyncRedis();

                                }
                            }
                            break;
                    }
                }

                _channel.BasicAck(msg.DeliveryTag, false);
            };

            await Task.Delay(1, stoppingToken);
            _channel.BasicConsume("CMSupdates", false, consumer);
        }

        private async Task ResyncRedis() 
        {
            var redisContentDb = redis.GetDatabase(1);
            redisContentDb.Execute("FLUSHDB");

            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var allContent = await context.Contents.ToArrayAsync();

                foreach (var content in allContent)
                {
                    await redisContentDb.StringSetAsync(content.Id.ToString(), JsonConvert.SerializeObject(content));
                }
            }
        }
    }
}
