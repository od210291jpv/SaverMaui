using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaverBackend.DTO;
using SaverBackend.Hubs;
using SaverBackend.Models;
using StackExchange.Redis;
using System.Linq;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SyncContentController : ControllerBase
    {
        private ApplicationContext db;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;

        private IHubContext<MainNotificationsHub> notificationsHubContext { get; set; }

        public SyncContentController(ApplicationContext database, IHubContext<MainNotificationsHub> hubcontext)
        {
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase(1);
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
                        Content newContent = new Content()
                        {
                            CategoryId = content.CategoryId,
                            ImageUri = content.ImageUri,
                            Title = content.Title,
                            Rating = content.Rating,
                        };

                        await db.Contents.AddAsync(newContent);
                    }
                }

                int result = await db.SaveChangesAsync();

                foreach (var content in contentRepresentation.Content) 
                {
                    Content newContent = new Content()
                    {
                        CategoryId = content.CategoryId,
                        ImageUri = content.ImageUri,
                        Title = content.Title,
                    };

                    var contentFromDb = await this.db.Contents.FirstOrDefaultAsync(c => c.ImageUri == newContent.ImageUri);

                    if (contentFromDb != null) 
                    {
                        await this.redisDb.StringSetAsync(contentFromDb.Id.ToString(), JsonConvert.SerializeObject(newContent));
                    }
                }

                if (result > 0) 
                {
                    return StatusCode(201);
                }

                await this.notificationsHubContext.Clients.All.SendAsync("SendNotificationsAsync", "Syncent content with client, but no updates where discovered.");

                return StatusCode(200);
            }

            return StatusCode(404);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddFavoriteContent(string login, string password, int[] contentIds) 
        {
            Profile? user = await this.db.Profiles.SingleOrDefaultAsync(p => p.UserName == login && p.Password == password);
            if (user == null) 
            {
                return NotFound("User not found");
            }

            foreach (var contentId in contentIds) 
            {
                var contentToBeAdded = await this.db.Contents.SingleOrDefaultAsync(c => c.Id == contentId);

                if (contentToBeAdded != null)
                {
                    if (await this.db.FavoriteContent.SingleOrDefaultAsync(f => f.FavoriteContentId == contentToBeAdded.Id && 
                    f.ProfileId == user.Id) is null )
                    {
                        user.FavoriteContent.Add(contentToBeAdded);
                    }
                }
            }

            var res = await this.db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> RemoveFavoriteContent(string login, string password, int contentId) 
        {
            Profile? user = await this.db.Profiles.SingleOrDefaultAsync(p => p.UserName == login && p.Password == password);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var contentToBeRemoved = this.db.FavoriteContent.SingleOrDefault(u => u.ProfileId == user.Id && u.FavoriteContentId == contentId);

            if (contentToBeRemoved is null) 
            {
                return NotFound("Content not found");
            }

            this.db.FavoriteContent.Remove(contentToBeRemoved);
            var result = await this.db.SaveChangesAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<int[]> GetFavoriteContent(string login, string password) 
        {
            Profile? user = await this.db.Profiles.SingleOrDefaultAsync(p => p.UserName == login && p.Password == password);
            if (user == null)
            {
                return Array.Empty<int>();
            }

            return this.db.FavoriteContent.Where(c => c.ProfileId == user.Id).Select(f => f.FavoriteContentId).ToArray();
        }

        [HttpPost("GetContentById")]
        public ContentDto[] GetContentById(int[] contentIds) 
        {
            var result = this.db.Contents.Where(c => contentIds.Contains(c.Id) == true);
            if (result is null) 
            {
                return Array.Empty<ContentDto>();
            }

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
            var content = await this.db.Contents.SingleOrDefaultAsync(c => c.Id == contentId);

            if(content is null) 
            {
                return NotFound($"Content with id {contentId} does not exists in db");
            }

            this.db.Contents.Remove(content);
            var deleteDbResult =  await this.db.SaveChangesAsync();
            var deleteRedisResult = await this.redisDb.KeyDeleteAsync(contentId.ToString());

            if (deleteDbResult == 0) 
            {
                return BadRequest("Coudn't delete the content from the DB");
            }
            if (deleteRedisResult == false) 
            {
                return BadRequest("Couldn't delete the content from Redis");
            }

            return Ok(content);
        }
    }
}
