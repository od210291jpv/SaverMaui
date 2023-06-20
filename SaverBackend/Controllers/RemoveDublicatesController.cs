using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RemoveDublicatesController : Controller
    {
        private ApplicationContext db;

        public RemoveDublicatesController(ApplicationContext database)
        {
            this.db = database;
        }

        [HttpGet(Name = "RemoveDublicates")]
        public async Task<IActionResult> Index()
        {
            var duplicatedContent = await this.db.Contents.GroupBy(v => v.ImageUri).Where(x => x.Count() > 1).Select(vd => vd.Key).ToArrayAsync();
            var duplicatedVideos = await this.db.Videos.GroupBy(v => v.VideoUri).Where(x => x.Count() > 1).Select(vd => vd.Key).ToArrayAsync();

            foreach (var item in duplicatedContent) 
            {
                var expectedContent = await this.db.Contents.Where(c => duplicatedContent!.Contains(c.ImageUri) == true).Skip(1).ToArrayAsync();
                this.db.Contents.RemoveRange(expectedContent);
            }

            foreach (var item in duplicatedVideos) 
            {
                var expectedVideos = await this.db.Videos.Where(v => duplicatedVideos!.Contains(v.VideoUri) == true).Skip(1).ToArrayAsync();
                this.db.Videos.RemoveRange(expectedVideos);
            }

            int result = await db.SaveChangesAsync();

            if (result > 0) 
            {
                return StatusCode(201, $"{result} duplicates where found");
            }
            return Ok(result);
        }
    }
}
