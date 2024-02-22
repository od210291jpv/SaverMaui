using BananasGamblerBackend.Database;
using BananasGamblerBackend.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BananasGamblerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private ApplicationContext database;

        public CategoriesController(ApplicationContext context)
        {
            this.database = context;
        }

        [HttpGet("/GetAllCategories")]
        public async Task<Category[]> Index()
        {
            var allCategories = await this.database.Categories.ToArrayAsync();
            return allCategories;
        }
    }
}
