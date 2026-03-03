using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaverBackend.Models;
using SaverBackend.Services.RabbitMq;
using StackExchange.Redis;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController : Controller
    {
        private ApplicationContext db;
        private IRabbitMqService mqService;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;
        private IDatabase redisContentDb;
        //private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public ContentController(ApplicationContext db, IRabbitMqService mq)
        {
            this.db = db;
            this.mqService = mq;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase();
            this.redisContentDb = redis.GetDatabase(1);
        }

        [HttpGet("UpdateContentRating")]
        public async Task<IActionResult> Index(int contentId, short rating, int? profileId)
        {
            //await this.webLogger.LogAsync($"Updating content rating for content with id {contentId} to {rating}", LogSeverity.Verbose);
            var content = this.db.Contents.SingleOrDefault(c => c.Id == contentId);
            if (content is null) 
            {
                //await this.webLogger.LogAsync($"No content with the ID {contentId} found", LogSeverity.Error);
                return NotFound($"No content with the ID {contentId}");
            }

            content.Rating = rating;
            var contentEntry = await this.redisContentDb.StringGetAsync(contentId.ToString());

            if (contentEntry.HasValue == true) 
            {
                var deserializedContent = JsonConvert.DeserializeObject<Content>(contentEntry);
                if (deserializedContent is not null) 
                {
                    deserializedContent.Rating = rating;
                    await this.redisContentDb.StringSetAsync(contentId.ToString(), JsonConvert.SerializeObject(deserializedContent));
                    //await this.webLogger.LogAsync($"Content with id {contentId} updated in Redis cache", LogSeverity.Verbose);
                }
            }

            if (profileId is not null) 
            {
                var user = this.db.Profiles.SingleOrDefault(c => c.Id == profileId);
                if (user is null) 
                {
                    //await this.webLogger.LogAsync($"Profile with id {profileId} not found", LogSeverity.Error);
                    return NotFound($"Profile with id {profileId} not found");
                }

                user.Funds += 30.0m;
                //await this.webLogger.LogAsync($"Profile with id {profileId} credited with 30.0 funds. Current user balance: {user.Funds}", LogSeverity.Verbose);
            }

            await this.db.SaveChangesAsync();
            //await this.webLogger.LogAsync($"Content with id {contentId} rating updated to {rating}", LogSeverity.Verbose);
            return Ok(content);
        }

        [HttpGet("initContentCost")]
        public async Task<IActionResult> InitContentCost() 
        {
            this.mqService.SendMessage("init", "InitContentQueue");
            //await this.webLogger.LogAsync("Init content cost message sent to InitContentQueue", LogSeverity.Verbose);
            return Ok();
        }

        [HttpGet("BuyContent")]
        public async Task<IActionResult> PurcahseContent(int userId, int contentId)
        {
            //await this.webLogger.LogAsync($"User with id {userId} is attempting to purchase content with id {contentId}", LogSeverity.Verbose);
            var content = this.db.Contents.SingleOrDefault<Content>(c => c.Id == contentId);
            if (content is null)
            {
                //await this.webLogger.LogAsync($"No content with the ID {contentId} found", LogSeverity.Error);
                return NotFound($"No content with the ID {contentId}");
            }

            //await this.webLogger.LogAsync($"Content with id {contentId} found. Cost: {content.Cost}", LogSeverity.Verbose);
            var user = this.db.Profiles.Single(p => p.Id == userId);
            if (user is null)
            {
                //await this.webLogger.LogAsync($" Purchase content No user with the ID {userId} found", LogSeverity.Error);
                return NotFound($"No user with the ID {userId} found");
            }

            if (user.Funds < content.Cost) 
            {
                //await this.webLogger.LogAsync($"User with id {userId} has insufficient funds. Current balance: {user.Funds}, content cost: {content.Cost}", LogSeverity.Warn);
                return BadRequest("The user has no enough funds");
            }

            user.Funds = user.Funds - content.Cost;

            if (content != null)
            {
                if (await this.db.FavoriteContent.SingleOrDefaultAsync(f => f.FavoriteContentId == content.Id &&
                f.ProfileId == user.Id) is null)
                {
                    user.FavoriteContent.Add(content);
                }
                //await this.webLogger.LogAsync($"User with id {userId} successfully purchased content with id {contentId}. New balance: {user.Funds}", LogSeverity.Verbose);
            }

            await this.db.SaveChangesAsync();
            //await this.webLogger.LogAsync($"User with id {userId} purchase process completed", LogSeverity.Verbose);
            return Ok();
        }

        [HttpGet("RandomContent")]
        public async Task<Content> GetRandomContent() 
        {
            //await this.webLogger.LogAsync("Fetching random content", LogSeverity.Verbose);
            var all = await this.db.Contents.ToArrayAsync();
            //await this.webLogger.LogAsync($"Total content items available: {all.Length}", LogSeverity.Verbose);
            Content random = all.ElementAt(new Random().Next(1, this.db.Contents.Count()));
            //await this.webLogger.LogAsync($"Random content selected: {random.Id} - {random.Title}", LogSeverity.Verbose);
            return random;
        }

        [HttpGet("ContentRating")]
        public async Task<short?> GetContentRating(int id) 
        {
            //await this.webLogger.LogAsync($"Fetching rating for content with id {id}", LogSeverity.Verbose);
            var content = await this.db.Contents.SingleOrDefaultAsync(c => c.Id == id);
            //await this.webLogger.LogAsync(content != null ? $"Content found. Rating: {content.Rating}" : $"No content with id {id} found", content != null ? LogSeverity.Verbose : LogSeverity.Warn);
            return content?.Rating;
        }

        [HttpGet("GetRatedContent")]
        public async Task<Content[]> GetRatedContent(short? rate) 
        {
            //await this.webLogger.LogAsync($"Fetching content with rating >= {rate}", LogSeverity.Verbose);
            var targetRating = rate != null ? rate.Value : 0;

            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).ToList() ?? new List<RedisKey>();

            RedisValue[] allValues = allKeys.AsParallel().Select(k => this.redisContentDb.StringGet(k)).Where(v => v.HasValue).ToArray();

            //await this.webLogger.LogAsync($"Total content items in Redis: {allValues.Length}", LogSeverity.Verbose);
            List<Content> result = new List<Content>();

            foreach (var value in allValues)
            {
                var deserialised = JsonConvert.DeserializeObject<Content>(value);
                if (deserialised != null && deserialised!.Rating >= targetRating)
                {
                    result.Add(deserialised);
                }
            }

            //await this.webLogger.LogAsync($"Total content items with rating >= {targetRating}: {result.Count}", LogSeverity.Verbose);
            return result.OrderByDescending(c => c.Rating).ToArray();
        }

        [HttpGet("GetAllContentCount")]
        public async Task<int> GetTotalcontentCount()
        {
            //await this.webLogger.LogAsync("Fetching total content count", LogSeverity.Verbose);
            return await this.db.Contents.CountAsync();
        }

        [HttpGet("SearchStatus")]
        public async Task<string> GetSearchStatus() 
        {             
            //await this.webLogger.LogAsync("Fetching search status from Redis", LogSeverity.Verbose);
            var redisSearchStateDb = this.redis.GetDatabase(6);
            //await this.webLogger.LogAsync("Connected to Redis database 6 for search status", LogSeverity.Verbose);
            var searchStatus = await redisSearchStateDb.StringGetAsync("SearchStatus");
            
            if (searchStatus.HasValue)
            {
                //await this.webLogger.LogAsync($"Search status found: {searchStatus}", LogSeverity.Verbose);
                return searchStatus.ToString();
            }

            //await this.webLogger.LogAsync("No search status found in Redis. Returning 'Inactive'", LogSeverity.Warn);
            return "Inactive";
        }
    }
}
