namespace SaverBackend.Services.RabbitMq
{
    public interface IRabbitMqService
    {
        void SendMessage(object obj);
        void SendMessage(string message, string queue);
        void SendMessage(object obj, string queue);
    }
}
