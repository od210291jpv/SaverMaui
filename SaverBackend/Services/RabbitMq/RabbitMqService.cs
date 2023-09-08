using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SaverBackend.Services.RabbitMq
{
    public class RabbitMqService : IRabbitMqService
    {
        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message, "");
        }

        public void SendMessage(string message, string queue)
        {
            var factory = new ConnectionFactory() { HostName = "192.168.88.55", UserName = "pi", Password = "raspberry" };
            using (var connection = factory.CreateConnection())
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
