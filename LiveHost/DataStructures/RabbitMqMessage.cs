namespace LiveHost.DataStructures
{
    public class RabbitMqMessage
    {
        public string Topic { get; set; } = string.Empty;

        public object[] Data { get; set; } = Array.Empty<object>();
    }
}
