using Microsoft.AspNetCore.Mvc;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private ApplicationContext db;

        public PingController(ApplicationContext database)
        {
            this.db = database;
        }

        [HttpGet("ping")]
        public async Task<bool> Index()
        {
            try
            {
                return await this.db.Database.CanConnectAsync();
            }
            catch 
            {
                return false;
            }
        }
    }
}
