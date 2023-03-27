using Microsoft.AspNetCore.Mvc;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadImageController : Controller
    {
        private ApplicationContext db;
        private readonly IWebHostEnvironment _env;

        private readonly string LocalImagesCategory = "LocalPublications";

        public UploadImageController(ApplicationContext database, IWebHostEnvironment env)
        {
            this.db = database;
            this._env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var host = HttpContext.Request.Host.ToUriComponent();

            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"{HttpContext.Request.Scheme}://{host}/{file.FileName}";

            if (!this.db.Categories.Any(c => c.Name == LocalImagesCategory)) 
            {
                var newCategory = new Category()
                {
                    CategoryId = Guid.NewGuid(),
                    Name = LocalImagesCategory,
                    AmountOfOpenings = 0,
                    AmountOfFavorites = 0,
                    ProfileId = 1
                };

                await db.Categories.AddAsync(newCategory);
            }

            await db.SaveChangesAsync();

            var category = this.db.Categories.Where(c => c.Name == LocalImagesCategory).FirstOrDefault();

            if (category is not null) 
            {
                await db.Contents.AddAsync(new Content()
                {
                    CategoryId = category.CategoryId,
                    ImageUri = fileUrl,
                    Title = $"LocallyStored{DateTime.Now.Day}",
                });
            }

            await db.SaveChangesAsync();

            return Ok(fileUrl);
        }

        [HttpGet("image/get/all")]
        public async Task<string> Index()
        {
            string contentRootPath = _env.ContentRootPath;
            string webRootPath = _env.WebRootPath;

            var host = HttpContext.Request.Host.ToUriComponent();
            var filepath = db.Images.Select(im => im.FileName).ToArray().FirstOrDefault();
            
            var url = $"{HttpContext.Request.Scheme}://{host}/{filepath}";

            return url;
        }
    }
}
