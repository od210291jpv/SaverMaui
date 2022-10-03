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

        public UploadImageController(ApplicationContext database, IWebHostEnvironment env)
        {
            this.db = database;
            this._env = env; 
        }

        [HttpPost("image")]
        public async Task<IActionResult> Index(IFormFile image)
        {
            string contentRootPath = _env.ContentRootPath;
            string webRootPath = _env.WebRootPath;

            string path = Path.Combine(webRootPath, "Images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }



            using (var ms = new MemoryStream())
            {
                image.CopyTo(ms);

                ImageModel img = new ImageModel()
                {
                    FileName = image.FileName,
                    Content = ms.ToArray()
                };

                using (FileStream stream = new FileStream(Path.Combine(path, image.FileName), FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                db.Images.Add(img);
                await db.SaveChangesAsync();
                return Json(img.Id);
            }
            
            return View("Index", image);
        }

        [HttpGet("image/get/all")]
        public async Task<Uri[]> Index()
        {
            string contentRootPath = _env.ContentRootPath;
            string webRootPath = _env.WebRootPath;


            var results =  db.Images.Select(im => new Uri(Path.Combine(webRootPath, "Images", im.FileName), UriKind.Relative)).ToArray();

            return results;
        }
    }
}
