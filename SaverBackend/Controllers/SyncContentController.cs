using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using SaverBackend.DTO;
using SaverBackend.Hubs;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SyncContentController : ControllerBase
    {
        private ApplicationContext db;
        private IHubContext<MainNotificationsHub> notificationsHubContext { get; set; }

        public SyncContentController(ApplicationContext database, IHubContext<MainNotificationsHub> hubcontext)
        {
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
                        await db.Contents.AddAsync(new Models.Content()
                        {
                            CategoryId = content.CategoryId,
                            ImageUri = content.ImageUri,
                            Title = content.Title,
                        });
                    }
                }

                int result = await db.SaveChangesAsync();

                if (result > 0) 
                {
                    var randomRecommendedCategory = contentRepresentation
                        .Categories
                        .Select(ct => ct.Name)
                        .ToArray()[new Random().Next(0, contentRepresentation.Categories.Length - 1)];

                    await this.notificationsHubContext.Clients.All.SendAsync("SendNotificationsAsync", "Looks like we have some new cool content for you!\n Check your feed for tasty updates!");
                    await this.notificationsHubContext.Clients.All.SendAsync("SendNotificationsAsync", $"Also Take a look at new hotty-notty content category '{randomRecommendedCategory.ToUpper()}', \n dont' forget to add to Favorites if you like it =)");

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
                    user.FavoriteContent.Add(contentToBeAdded);
                }
            }

            var res = await this.db.SaveChangesAsync();
            return Ok();
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

            return this.db.Contents.Where(c => c.ProfileId == user.Id).Select(c => c.Id).ToArray();
        }

        [HttpGet("GetContentById")]
        public async Task<ContentDto?> GetContentById(int contentId) 
        {
            var result = await this.db.Contents.SingleOrDefaultAsync(c => c.Id == contentId);
            if (result is null) 
            {
                return default(ContentDto?);
            }

            return new ContentDto()
            {
                CategoryId = result.CategoryId,
                ImageUri = result.ImageUri,
                Title = result.Title
            };
        }
    }
}
