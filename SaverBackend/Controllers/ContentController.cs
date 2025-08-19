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
            var content = this.db.Contents.SingleOrDefault(c => c.Id == contentId);
            if (content is null) 
            {
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
                }
            }

            if (profileId is not null) 
            {
                var user = this.db.Profiles.SingleOrDefault(c => c.Id == profileId);
                if (user is null) 
                {
                    return NotFound($"Profile with id {profileId} not found");
                }

                user.Funds += 30.0m;
            }

            await this.db.SaveChangesAsync();
            return Ok(content);
        }

        [HttpGet("initContentCost")]
        public IActionResult InitContentCost() 
        {
            this.mqService.SendMessage("init", "InitContentQueue");
            return Ok();
        }

        [HttpGet("BuyContent")]
        public async Task<IActionResult> PurcahseContent(int userId, int contentId)
        {
            var content = this.db.Contents.SingleOrDefault<Content>(c => c.Id == contentId);
            if (content is null)
            {
                return NotFound($"No content with the ID {contentId}");
            }

            var user = this.db.Profiles.Single(p => p.Id == userId);
            if (user is null)
            {
                return NotFound($"No user with the ID {contentId}");
            }

            if (user.Funds < content.Cost) 
            {
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
            }

            await this.db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("RandomContent")]
        public async Task<Content> GetRandomContent() 
        {
            var all = await this.db.Contents.ToArrayAsync();
            Content random = all.ElementAt(new Random().Next(1, this.db.Contents.Count()));
            return random;
        }

        [HttpGet("ContentRating")]
        public async Task<short?> GetContentRating(int id) 
        {
            var content = await this.db.Contents.SingleOrDefaultAsync(c => c.Id == id);
            return content?.Rating;
        }

        [HttpGet("GetRatedContent")]
        public Content[] GetRatedContent(short? rate) 
        {
            var targetRating = rate != null ? rate.Value : 0;

            List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).ToList() ?? new List<RedisKey>();

            RedisValue[] allValues = allKeys.AsParallel().Select(k => this.redisContentDb.StringGet(k)).Where(v => v.HasValue).ToArray();

            List<Content> result = new List<Content>();

            foreach (var value in allValues)
            {
                var deserialised = JsonConvert.DeserializeObject<Content>(value);
                if (deserialised != null && deserialised!.Rating >= targetRating)
                {
                    result.Add(deserialised);
                }
            }

            return result.OrderByDescending(c => c.Rating).ToArray();
        }

        [HttpGet("GetAllContentCount")]
        public async Task<int> GetTotalcontentCount()
        {
            return await this.db.Contents.CountAsync();
        }
    }
}
