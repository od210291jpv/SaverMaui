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
    public class CreateCategoryController : ControllerBase
    {
        private ApplicationContext db;
        private IHubContext<MainNotificationsHub> notificationsHubContext { get; set; }

        public CreateCategoryController(ApplicationContext db, IHubContext<MainNotificationsHub> notificationsHubContext)
        {
            this.db = db;
            this.notificationsHubContext = notificationsHubContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Index(CategoryDto category)
        {
            if (!db.Categories.Select(ct => ct.CategoryId).ToArray().Contains(category.CategoryId)) 
            {
                var newCategory = new Category()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    AmountOfOpenings = category.AmountOfOpenings ?? 0,
                    AmountOfFavorites = category.AmountOfFavorites ?? 0,                    
                };

                await this.db.Categories.AddAsync(newCategory);

                if (category.PublisherProfileId is not null) 
                {
                    Profile? profile =  await this.db.Profiles.SingleOrDefaultAsync(p => p.ProfileId == category.PublisherProfileId);

                    if (profile?.PublishedCategories is not null)
                    {
                        profile.PublishedCategories.Add(newCategory);
                    }
                    else 
                    {
                        profile.PublishedCategories = new List<Category>();
                        profile.PublishedCategories.Add(newCategory);
                    }
                }

                await this.db.SaveChangesAsync();
                await this.notificationsHubContext.Clients.All.SendAsync($"New Category {category.Name} added!");
                return StatusCode(201);

            }

            return StatusCode(200);
        }
    }
}
