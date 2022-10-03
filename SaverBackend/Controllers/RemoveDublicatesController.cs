using Microsoft.AspNetCore.Mvc;
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

            this.db.Remove(this.db.Contents.Where(ct => this.db.Contents.Select(cct => cct.ImageUri).ToArray().Length > 1).First());

            int result = await db.SaveChangesAsync();

            if (result > 0) 
            {
                return StatusCode(201, $"{result} duplicates where found");
            }
            return Ok(result);
        }
    }
}
