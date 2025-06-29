
namespace ContentParserBackend.Services
{
    public interface ISerachEngine
    {
        Task<List<string>> ParseAsync(string keyword, IRabbitMqService mqClient);
    }
}