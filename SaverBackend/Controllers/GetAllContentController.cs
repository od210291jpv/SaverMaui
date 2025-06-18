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
        public async Task<Content[]> Index()
        {
            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).ToList() ?? new List<RedisKey>();

            Content?[] all =  allKeys.AsParallel().Select(k => JsonConvert.DeserializeObject<Content>(this.redisDb.StringGet(k))).ToArray();

            return all;
        }

        [HttpGet("GetAllContentPaged")]
        public async Task<Content[]> GetAllContentPaged(short page = 0, short pageSize = 200) 
        {
            int howManyToSkip = 0;
            
            if (page > 0) 
            {
                howManyToSkip = pageSize * page;
            }

            RedisKey[] allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).Skip(howManyToSkip).Take(pageSize).ToArray();

            List<Content> results = new(allKeys.Length);

            foreach (RedisKey key in allKeys) 
            {
                var redisValue = await this.redisDb.StringGetAsync(key);
                if (redisValue.HasValue) 
                {
                    var deserialized = JsonConvert.DeserializeObject<Content>(redisValue);
                    if (deserialized != null) 
                    {
                        results.Add(deserialized);
                    }
                }
            }

            var sorted = results.OrderBy(r => r.Id).Reverse();

            return sorted.ToArray();
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
