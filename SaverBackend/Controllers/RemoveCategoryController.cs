using Microsoft.AspNetCore.Mvc;

using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RemoveCategoryController : ControllerBase
    {
        private ApplicationContext db;

        public RemoveCategoryController(ApplicationContext database)
        {
            this.db = database;
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Index(Guid categoryId)
        {
            var requiredCategory = this.db.Categories.Where(ct => ct.CategoryId == categoryId).FirstOrDefault();

            if (requiredCategory != null) 
            {
                var relatedContent = this.db.Contents.Where(ct => ct.CategoryId == categoryId).ToArray();
                this.db.Contents.RemoveRange(relatedContent);

                db.Categories.Remove(requiredCategory);
                var result = await db.SaveChangesAsync();

                return StatusCode(200);
            }

            return StatusCode(404, $"Required category with ID {categoryId} not fouond");
        }
    }
}
