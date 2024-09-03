using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaverBackend.DTO;
using SaverBackend.Models;
using StackExchange.Redis;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetAllContentController : Controller
    {
        private ApplicationContext db;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;

        public GetAllContentController(ApplicationContext database)
        {
            this.db = database;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase(1);
        }

        [HttpGet(Name = "GetAllContent")]
        public async Task<ContentDto[]> Index()
        {
            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).ToList() ?? new List<RedisKey>();

            var allContent = new List<ContentDto>();
            var result = allKeys.AsParallel().Select(k => JsonConvert.DeserializeObject<ContentDto>(this.redisDb.StringGet(k))).ToArray();

            return result;
        }
    }
}
