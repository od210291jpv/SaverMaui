using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaverBackend.DTO;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetAllContentController : Controller
    {
        private ApplicationContext db;

        public GetAllContentController(ApplicationContext database)
        {
            this.db = database;
        }

        [HttpGet(Name = "GetAllContent")]
        public async Task<ContentDto[]> Index()
        {
            var allContent =  await this.db.Contents.ToArrayAsync();

            return allContent.Select(ct => new ContentDto() 
            {
                CategoryId = ct.CategoryId,
                ImageUri = ct.ImageUri,
                Title = ct.Title
            }).ToArray();
        }
    }
}
