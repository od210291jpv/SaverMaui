using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using BananasGamblerBackend.Database;
//using SaverBackend.Models;
using System.Linq;

namespace BananasGamblerBackend.Services
{
    public class RabbitMqListener : BackgroundService
    {

        private IConnection _connection;
        private IModel _channel;

        private readonly IServiceScopeFactory serviceScopeFactory;

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = "192.168.88.55", UserName = "pi", Password = "raspberry" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "CardsUpdateQueque",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                               routingKey: "CardsUpdateQueque",
                               basicProperties: null,
                               body: body);
            }
        }

        public RabbitMqListener(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory { HostName = "192.168.88.55", UserName = "pi", Password = "raspberry" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "CardsUpdateQueque", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());


                this.PullNewGameCards();

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("CardsUpdateQueque", false, consumer);

            return Task.CompletedTask;
        }

        private async void PullNewGameCards()
        {
            using var scope = serviceScopeFactory.CreateScope();

            using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            using var saverNackendContext = scope.ServiceProvider.GetRequiredService<SaveBackendApplicationContext>();

            var existingCards = context.GameCards.ToArray();
            var saverCards = saverNackendContext.Contents.ToArray();
            var saverCardsUrls = saverNackendContext.Contents.Select(c => c.ImageUri);
            var existingCardsUrls = existingCards.Select(c => c.ImageUri).ToArray();

            var cardsToBeAdded = saverCards.Where(c => existingCardsUrls.Contains(c.ImageUri) == false).ToArray();

            foreach (var card in cardsToBeAdded)
            {
                await context.GameCards.AddAsync(new Database.Models.GameCard()
                {
                    CardTitle = card.Title,
                    CostInCredits = new Random().Next(1, 150),
                    ImageUri = card.ImageUri,
                    DateCreated = DateTime.UtcNow,
                    Rarity = new Random().Next(1, 100),
                });

                await context.SaveChangesAsync();
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
