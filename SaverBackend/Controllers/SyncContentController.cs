using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using SaverBackend.DTO;
using SaverBackend.Hubs;
using SaverBackend.Models;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;
using WebLoggerClient;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SyncContentController : ControllerBase
    {
        private ApplicationContext db;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;
        private IDatabase LatestUpdatesRedisDb;
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        private IHubContext<MainNotificationsHub> notificationsHubContext { get; set; }

        public SyncContentController(ApplicationContext database, IHubContext<MainNotificationsHub> hubcontext)
        {
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase(1);
            this.LatestUpdatesRedisDb = redis.GetDatabase(4);
            this.db = database;
            this.notificationsHubContext = hubcontext;
        }

        [HttpPost(Name = "SyncContent")]
        public async Task<IActionResult> SyncContent(ContentRepresentationData contentRepresentation)
        {
            if (contentRepresentation is not null) 
            {
                foreach (var category in contentRepresentation.Categories) 
                {
                    if (await db.Categories.Where(c => c.CategoryId == category.CategoryId).CountAsync().ConfigureAwait(false) == 0) 
                    {
                        var newCategory = new Category()
                        {
                            CategoryId = category.CategoryId,
                            Name = category.Name,
                            AmountOfOpenings = category.AmountOfOpenings,
                            AmountOfFavorites = category.AmountOfFavorites,                            
                        };

                        if (category.PublisherProfileId != null) 
                        {
                            var publisherProfile = await this.db.Profiles.SingleAsync(pr => pr.ProfileId == category.PublisherProfileId);
                            newCategory.Profile = publisherProfile;
                        }

                        await this.db.Categories.AddAsync(newCategory);
                    }
                }

                foreach (var content in contentRepresentation.Content)
                {
                    if (this.db.Contents.Where(ct => ct.ImageUri == content.ImageUri && ct.CategoryId == content.CategoryId).Count() == 0) 
                    {
                        List<RedisKey> allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(1).ToList() ?? new List<RedisKey>();

                        var newId = allKeys.AsParallel().Select(k => JsonConvert.DeserializeObject<Content>(this.redisDb.StringGet(k))).Select(c => c.Id).Max() + 2;

                        Content newContent = new Content()
                        {
                            CategoryId = content.CategoryId,
                            ImageUri = content.ImageUri,
                            Title = content.Title,
                            Rating = content.Rating,
                            Id = newId,
                        };

                        await db.Contents.AddAsync(newContent);
                        await this.redisDb.StringSetAsync(newId.ToString(), JsonConvert.SerializeObject(newContent));
                        await this.AddContentIntoLatest(new List<Content>() { newContent });
                    }
                }

                int result = await db.SaveChangesAsync();

                if (result > 0) 
                {
                    return StatusCode(201);
                }

                await this.notificationsHubContext.Clients.All.SendAsync("SendNotificationsAsync", "Syncent content with client, but no updates where discovered.");

                return StatusCode(200);
            }

            return StatusCode(404);
        }

        private async Task AddContentIntoLatest(List<Content> content) 
        {
            RedisKey[] allKeys = this.redis.GetServer("192.168.88.252:6379").Keys(4).ToArray();
            if (content.Count >= 100) 
            {
                foreach (var k in allKeys) 
                {
                    await this.LatestUpdatesRedisDb.KeyDeleteAsync(k);
                }

                foreach (var con in content) 
                {
                    await this.redisDb.StringSetAsync(con.Id.ToString(), JsonConvert.SerializeObject(con));
                }

                return;
            }

            int contentToUpdate = 100 - content.Count;

            if (allKeys.Length >= 100) 
            {
                var keysToDelete = allKeys.Take(contentToUpdate).ToArray();

                foreach (var k in keysToDelete)
                {
                    await this.LatestUpdatesRedisDb.KeyDeleteAsync(k);
                }

                foreach (var con in content)
                {
                    await this.LatestUpdatesRedisDb.StringSetAsync(con.Id.ToString(), JsonConvert.SerializeObject(con));
                }
            }
            return;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddFavoriteContent(string login, string password, int[] contentIds) 
        {
            await this.webLogger.LogAsync($"Attempting to add favorite content for user {login}", LogSeverity.Verbose);
            Profile? user = await this.db.Profiles.SingleOrDefaultAsync(p => p.UserName == login && p.Password == password);
            if (user == null) 
            {
                await this.webLogger.LogAsync($"User {login} not found with provided credentials", LogSeverity.Warn);
                return NotFound("User not found");
            }

            await this.webLogger.LogAsync($"User {login} found. Proceeding to add favorite content.", LogSeverity.Verbose);
            foreach (var contentId in contentIds) 
            {
                await this.webLogger.LogAsync($"Processing content ID {contentId} for user {login}", LogSeverity.Verbose);
                var contentToBeAdded = await this.db.Contents.SingleOrDefaultAsync(c => c.Id == contentId);

                await this.webLogger.LogAsync(contentToBeAdded != null ? 
                    $"Content with ID {contentId} found. Checking if it's already a favorite for user {login}." : 
                    $"Content with ID {contentId} not found in database.", LogSeverity.Verbose);
                if (contentToBeAdded != null)
                {
                    if (await this.db.FavoriteContent.SingleOrDefaultAsync(f => f.FavoriteContentId == contentToBeAdded.Id && 
                    f.ProfileId == user.Id) is null )
                    {
                        await this.webLogger.LogAsync($"Content with ID {contentId} is not already a favorite for user {login}. Adding to favorites.", LogSeverity.Verbose);
                        user.FavoriteContent.Add(contentToBeAdded);
                    }
                }
            }

            await this.webLogger.LogAsync($"Saving changes to database for user {login}", LogSeverity.Verbose);
            _ = await this.db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> RemoveFavoriteContent(string login, string password, int contentId) 
        {
            await this.webLogger.LogAsync($"Attempting to remove favorite content for user {login}", LogSeverity.Verbose);
            Profile? user = await this.db.Profiles.SingleOrDefaultAsync(p => p.UserName == login && p.Password == password);
            if (user == null)
            {
                await this.webLogger.LogAsync($"User {login} not found with provided credentials", LogSeverity.Warn);
                return NotFound("User not found");
            }

            await this.webLogger.LogAsync($"User {login} found. Proceeding to remove favorite content.", LogSeverity.Verbose);
            var contentToBeRemoved = this.db.FavoriteContent.SingleOrDefault(u => u.ProfileId == user.Id && u.FavoriteContentId == contentId);

            await this.webLogger.LogAsync(contentToBeRemoved != null ?
                $"Content with ID {contentId} found in favorites for user {login}. Proceeding to remove." :
                $"Content with ID {contentId} not found in favorites for user {login}.", LogSeverity.Verbose);
            if (contentToBeRemoved is null) 
            {
                await this.webLogger.LogAsync($"No favorite content with ID {contentId} found for user {login}. Cannot remove.", LogSeverity.Warn);
                return NotFound("Content not found");
            }

            await this.webLogger.LogAsync($"Removing content with ID {contentId} from favorites for user {login}.", LogSeverity.Verbose);
            this.db.FavoriteContent.Remove(contentToBeRemoved);
            var result = await this.db.SaveChangesAsync();

            await this.webLogger.LogAsync($"Content with ID {contentId} removed from favorites for user {login}. Database save result: {result}", LogSeverity.Verbose);
            return Ok(result);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<int[]> GetFavoriteContent(string login, string password) 
        {
            await this.webLogger.LogAsync($"Fetching favorite content for user {login}", LogSeverity.Verbose);
            Profile? user = await this.db.Profiles.SingleOrDefaultAsync(p => p.UserName == login && p.Password == password);
            await this.webLogger.LogAsync(user != null ?
                $"User {login} found. Retrieving favorite content." :
                $"User {login} not found with provided credentials.", LogSeverity.Verbose);
            if (user == null)
            {
                await this.webLogger.LogAsync($"No user found for {login}. Cannot retrieve favorite content.", LogSeverity.Warn);
                return Array.Empty<int>();
            }

            await this.webLogger.LogAsync($"User {login} found. Favorite content retrieval in progress.", LogSeverity.Verbose);
            return this.db.FavoriteContent.Where(c => c.ProfileId == user.Id).Select(f => f.FavoriteContentId).ToArray();
        }

        [HttpPost("GetContentById")]
        public async Task<ContentDto[]> GetContentById(int[] contentIds) 
        {
            await this.webLogger.LogAsync($"Fetching content for provided IDs: {string.Join(", ", contentIds)}", LogSeverity.Verbose);
            var result = this.db.Contents.Where(c => contentIds.Contains(c.Id) == true);
            await this.webLogger.LogAsync($"Total content items found for provided IDs: {result.Count()}", LogSeverity.Verbose);
            if (result is null) 
            {
                await this.webLogger.LogAsync("No content found for the provided IDs.", LogSeverity.Warn);
                return Array.Empty<ContentDto>();
            }

            await this.webLogger.LogAsync("Mapping content to ContentDto", LogSeverity.Verbose);
            return result.AsParallel().Select(c => new ContentDto() 
            {
                CategoryId = c.CategoryId,
                ImageUri = c.ImageUri,
                Title = c.Title,
                Id = c.Id,
                Cost = c.Cost,
                Rating = c.Rating,
            }).ToArray();
        }

        [HttpDelete("DeleteContent")]
        public async Task<IActionResult> RemoveContent(int contentId) 
        {
            await this.webLogger.LogAsync($"Attempting to delete content with ID {contentId}", LogSeverity.Verbose);
            var content = await this.db.Contents.SingleOrDefaultAsync(c => c.Id == contentId);

            await this.webLogger.LogAsync(content != null ? 
                $"Content with ID {contentId} found. Proceeding to delete." : 
                $"Content with ID {contentId} not found in database.", LogSeverity.Verbose);
            if (content is null) 
            {
                await this.webLogger.LogAsync($"No content with ID {contentId} exists in the database. Cannot delete.", LogSeverity.Error);
                return NotFound($"Content with id {contentId} does not exists in db");
            }

            await this.webLogger.LogAsync($"Deleting content with ID {contentId} from database and Redis cache.", LogSeverity.Verbose);
            this.db.Contents.Remove(content);
            var deleteDbResult =  await this.db.SaveChangesAsync();
            var deleteRedisResult = await this.redisDb.KeyDeleteAsync(contentId.ToString());

            await this.webLogger.LogAsync($"Content with ID {contentId} deletion results - Database: {deleteDbResult}, Redis: {deleteRedisResult}", LogSeverity.Verbose);
            if (deleteDbResult == 0) 
            {
                await this.webLogger.LogAsync($"Failed to delete content with ID {contentId} from database.", LogSeverity.Error);
                return BadRequest("Coudn't delete the content from the DB");
            }
            if (deleteRedisResult == false) 
            {
                await this.webLogger.LogAsync($"Failed to delete content with ID {contentId} from Redis.", LogSeverity.Error);
                return BadRequest("Couldn't delete the content from Redis");
            }

            await this.webLogger.LogAsync($"Content with ID {contentId} successfully deleted from both database and Redis.", LogSeverity.Verbose);
            return Ok(content);
        }
    }
}
