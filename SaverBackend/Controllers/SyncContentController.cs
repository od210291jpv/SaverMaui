using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
        public async Task<IActionResult> Index(ContentRepresentationData contentRepresentation)
        {
            if (contentRepresentation is not null) 
            {
                foreach (var category in contentRepresentation.Categories) 
                {
                    if (db.Categories.Where(c => c.CategoryId == category.CategoryId).Count() == 0) 
                    {
                        await this.db.Categories.AddAsync(new Category()
                        {
                            CategoryId = category.CategoryId,
                            Name = category.Name,
                            AmountOfOpenings = category.AmountOfOpenings,
                            AmountOfFavorites = category.AmountOfFavorites,
                        });
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
    }
}
