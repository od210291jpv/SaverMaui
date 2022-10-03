using Microsoft.AspNetCore.Mvc;
using SaverBackend.DTO;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateCategoryStatisticsController : Controller
    {
        private ApplicationContext db;

        public UpdateCategoryStatisticsController(ApplicationContext appContext)
        {
            this.db = appContext;
        }

        [HttpPost(Name = "UpdateCategoryStatistics")]
        public async Task<IActionResult> Index(UpdateStatisticsDto dtoData)
        {
            Category requestedCategory;

            if (!this.db.Categories.Select(ct => ct.CategoryId).ToArray().Contains<Guid>(dtoData.CategoryId)) 
            {
                return NotFound();
            }

            requestedCategory = this.db.Categories.Single(ct => ct.CategoryId == dtoData.CategoryId);

            requestedCategory.AmountOfFavorites = dtoData.Favorites;
            requestedCategory.AmountOfOpenings = dtoData.Openings;

            await this.db.SaveChangesAsync();

            return Ok(
                new CategoryDto() 
                {
                    CategoryId = requestedCategory.CategoryId,
                    Name = requestedCategory.Name,
                    AmountOfFavorites = requestedCategory.AmountOfFavorites,
                    AmountOfOpenings = requestedCategory.AmountOfOpenings 
                });
        }
    }
}
