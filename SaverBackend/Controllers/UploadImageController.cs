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
        private readonly string SambaPath = @"\\192.168.0.137\mainShare\renders";

        public UploadImageController(ApplicationContext database, IWebHostEnvironment env)
        {
            this.db = database;
            this._env = env;
        }

        [HttpPost("image")]
        public async Task<IActionResult> Index(IFormFile image)
        {
            string webRootPath = _env.WebRootPath;

            string path = Path.Combine(webRootPath, "Images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string sambaFilePath = Path.Combine(SambaPath, image.FileName);

            if (Directory.Exists(SambaPath)) 
            {
                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);

                    ImageModel img = new ImageModel()
                    {
                        FileName = image.FileName,
                        Content = ms.ToArray()
                    };

                    using (FileStream stream = new FileStream(sambaFilePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    db.Images.Add(img);
                    await db.SaveChangesAsync();
                    return Json(img.Id);
                }
            }

            return Json("Looks Like samba share is not mounted");
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
