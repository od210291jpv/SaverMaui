using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace ContentParserBackend.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        private IConnection connection;

        public RabbitMqService()
        {
            var factory = new ConnectionFactory() { HostName = "192.168.88.252", UserName = "pi", Password = "raspberry" };
            connection = factory.CreateConnection();
        }

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message, "");
        }

        public void SendMessage(string message, string queue)
        {
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                               routingKey: queue,
                               basicProperties: null,
                               body: body);
            }
        }

        public void SendMessage(object obj, string queue)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message, queue);
        }
    }
}
