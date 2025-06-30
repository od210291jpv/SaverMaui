using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaverBackend.DTO;
using SaverBackend.Models;
using StackExchange.Redis;
using System.Net;
using Tensorflow;

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

            var allPagedKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).AsParallel().OrderBy(k => int.Parse(k)).Skip(howManyToSkip).Take(pageSize).ToArray();

            var allValues = new List<Content>();

            foreach (var k in allPagedKeys) 
            {
                var redisValue = await this.redisDb.StringGetAsync(k);
                if (redisValue.HasValue)
                {
                    var rr = JsonConvert.DeserializeObject<Content>(redisValue);

                    if (rr is not null) 
                    {
                        allValues.add(rr);
                    }
                }
            }

            return allValues.OrderBy(v => v.Id).ToArray();
        }

        [HttpGet("searchResults")]
        public string[] GetSearchResults() 
        {
            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(2).ToList() ?? new List<RedisKey>();
            var resultsDb = this.redis.GetDatabase(2);

            return allKeys.AsParallel().Select(k => resultsDb.StringGet(k).ToString()).Distinct().ToArray();
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
