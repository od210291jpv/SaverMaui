using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaverBackend.DTO;
using SaverBackend.Models;
using StackExchange.Redis;
using System.Net;

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

            return allKeys.AsParallel().Select(k => JsonConvert.DeserializeObject<ContentDto>(this.redisDb.StringGet(k))).ToArray();
        }

        [HttpGet("searchResults")]
        public string[] GetSearchResults() 
        {
            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(2).ToList() ?? new List<RedisKey>();
            var resultsDb = this.redis.GetDatabase(2);

            return allKeys.AsParallel().Select(k => resultsDb.StringGet(k).ToString()).ToArray();
        }

        [HttpDelete("CleanResults")]
        public HttpStatusCode CleanSerchResults() 
        {
            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(2).ToList() ?? new List<RedisKey>();
            var resultsDb = this.redis.GetDatabase(2);

            allKeys.AsParallel().Select(k => resultsDb.StringGetDelete(k).ToString()).ToArray();
            return HttpStatusCode.OK;
        }
    }
}
