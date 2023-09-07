using Microsoft.AspNetCore.Mvc;

namespace BananasGamblerBackend.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
