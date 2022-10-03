using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaverBackend.DTO;
using SaverBackend.Models;
using System.Linq;

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
    }
}
