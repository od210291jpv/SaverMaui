using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaverBackend.Models;
using StackExchange.Redis;
using System.Net;
using System.Threading.Tasks;
using Tensorflow;
using WebLoggerClient;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetAllContentController : Controller
    {
        private ApplicationContext db;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;
        private IDatabase LatestUpdatesRedisDb;
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public GetAllContentController(ApplicationContext database)
        {
            this.db = database;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase(1);
            this.LatestUpdatesRedisDb = redis.GetDatabase(4);
        }

        [HttpGet(Name = "GetAllContent")]
        public async Task<Content[]> Index()
        {
            await this.webLogger.LogAsync("Getting all content from Redis database", LogSeverity.Verbose);
            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).ToList() ?? new List<RedisKey>();
            await this.webLogger.LogAsync($"Total keys found in Redis database 1: {allKeys.Count}", LogSeverity.Verbose);
            Content?[] all =  allKeys.AsParallel().Select(k => JsonConvert.DeserializeObject<Content>(this.redisDb.StringGet(k))).ToArray();
            await this.webLogger.LogAsync($"Total content items retrieved: {all.Length}", LogSeverity.Verbose);
            return all;
        }

        [HttpGet("GetAllContentPaged")]
        public async Task<Content[]> GetAllContentPaged(short page = 0, short pageSize = 200) 
        {
            await this.webLogger.LogAsync($"Getting all content from Redis database, page {page}, page size {pageSize}", LogSeverity.Verbose);
            int howManyToSkip = 0;
            
            await this.webLogger.LogAsync("Calculating keys to skip based on page and page size", LogSeverity.Verbose);
            if (page > 0) 
            {
                howManyToSkip = pageSize * page;
                await this.webLogger.LogAsync($"Calculated keys to skip: {howManyToSkip}", LogSeverity.Verbose);
            }

            await this.webLogger.LogAsync("Fetching paged keys from Redis", LogSeverity.Verbose);
            var allPagedKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).AsParallel().OrderByDescending(k => int.Parse(k)).Skip(howManyToSkip).Take(pageSize).ToArray();
            await this.webLogger.LogAsync($"Total paged keys fetched: {allPagedKeys.Length}", LogSeverity.Verbose);

            var allValues = new List<Content>();

            await this.webLogger.LogAsync("Fetching content for each paged key", LogSeverity.Verbose);
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

            await this.webLogger.LogAsync($"Total content items retrieved for page {page}: {allValues.Count}", LogSeverity.Verbose);

            return allValues.OrderByDescending(v => v.DateCreated).ToArray();
        }

        [HttpGet("GetLatestContent")]
        public async Task<Content[]> GetLatestContent() 
        {
            var allValues = new List<Content>();
            var allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(4).ToArray();

            foreach (var k in allKeys)
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

            return allValues.OrderByDescending(v => v.DateCreated).ToArray();
        }

        [HttpGet("searchResults")]
        public async Task<string[]> GetSearchResults() 
        {
            await this.webLogger.LogAsync("Fetching all search results from Redis database 2", LogSeverity.Verbose);
            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(2).ToList() ?? new List<RedisKey>();
            var resultsDb = this.redis.GetDatabase(2);
            await this.webLogger.LogAsync($"Total keys found in Redis database 2: {allKeys.Count}", LogSeverity.Verbose);
            return allKeys.AsParallel().Select(k => resultsDb.StringGet(k).ToString()).Distinct().ToArray();
        }

        [HttpDelete("CleanResults")]
        public async Task<HttpStatusCode> CleanSerchResults() 
        {
            await this.webLogger.LogAsync("Cleaning all search results from Redis database 2", LogSeverity.Verbose);
            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(2).ToList() ?? new List<RedisKey>();
            var resultsDb = this.redis.GetDatabase(2);

            await this.webLogger.LogAsync($"Total keys to delete in Redis database 2: {allKeys.Count}", LogSeverity.Verbose);
            allKeys.AsParallel().Select(k => resultsDb.StringGetDelete(k).ToString()).ToArray();
            await this.webLogger.LogAsync("All search results cleaned from Redis database 2", LogSeverity.Verbose);
            return HttpStatusCode.OK;
        }
    }
}
