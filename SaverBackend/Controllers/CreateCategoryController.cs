using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SaverBackend.DTO;

using SaverBackend.Hubs;

using SaverBackend.Models;
using WebLoggerClient;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateCategoryController : ControllerBase
    {
        private ApplicationContext db;
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");
        private IHubContext<MainNotificationsHub> notificationsHubContext { get; set; }

        public CreateCategoryController(ApplicationContext db, IHubContext<MainNotificationsHub> notificationsHubContext)
        {
            this.db = db;
            this.notificationsHubContext = notificationsHubContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Index(CategoryDto category)
        {
            await this.webLogger.LogAsync($"Creating category with id {category.CategoryId} and name {category.Name}", LogSeverity.Verbose);
            if (!db.Categories.Select(ct => ct.CategoryId).ToArray().Contains(category.CategoryId)) 
            {
                await this.webLogger.LogAsync($"No category with id {category.CategoryId} found, creating new one", LogSeverity.Verbose);
                var newCategory = new Category()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    AmountOfOpenings = category.AmountOfOpenings ?? 0,
                    AmountOfFavorites = category.AmountOfFavorites ?? 0,
                };

                await this.webLogger.LogAsync($"Category object created, checking for publisher profile with id {category.PublisherProfileId}", LogSeverity.Verbose);

                if (category.PublisherProfileId != null)
                {
                    var publisherProfile = await this.db.Profiles.SingleOrDefaultAsync(pr => pr.ProfileId == category.PublisherProfileId);
                    newCategory.Profile = publisherProfile;
                }


                await this.webLogger.LogAsync($"Adding new category to database", LogSeverity.Verbose);
                await this.db.Categories.AddAsync(newCategory);

                await this.db.SaveChangesAsync();
                await this.webLogger.LogAsync($"New category with id {category.CategoryId} added to database", LogSeverity.Verbose);
                await this.notificationsHubContext.Clients.All.SendAsync($"New Category {category.Name} added!");
                await this.webLogger.LogAsync($"Notification about new category {category.Name} sent to all clients", LogSeverity.Verbose);
                return StatusCode(201);
            }

            await this.webLogger.LogAsync($"Category with id {category.CategoryId} already exists, skipping creation", LogSeverity.Warn);
            return StatusCode(200);
        }
    }
}
