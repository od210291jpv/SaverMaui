using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SaverBackend.DTO;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetCategoriesController : ControllerBase
    {

        private ApplicationContext db;

        public GetCategoriesController(ApplicationContext database)
        {
            this.db = database;
        }

        [HttpGet(Name = "GetCategories")]
        public async Task<CategoryDto[]> Index()
        {
            Category[]? allCategories = await db.Categories.ToArrayAsync();

            IEnumerable<CategoryDto>? results = allCategories?.Select(ct => new CategoryDto() 
            {
                Name = ct.Name,
                CategoryId = ct.CategoryId,                
            });

            return results?.ToArray();
        }

        [HttpPost(Name = "GetPopularCategories")]
        public async Task<CategoryDto[]> GetMostPopularCategories(int limit)
        {
            var result = this.db.Categories.OrderByDescending(ct => ct.AmountOfOpenings).ToArray();

            CategoryDto[] limitedResult = new CategoryDto[limit];

            for (int i = 0; i != limit; i++)
            {
                limitedResult[i] = new CategoryDto()
                {
                    Name = result[i].Name,
                    CategoryId = result[i].CategoryId,
                    AmountOfFavorites = result[i].AmountOfFavorites,
                    AmountOfOpenings = result[i].AmountOfOpenings,
                };
            }
            return limitedResult;
        }
    }
}
