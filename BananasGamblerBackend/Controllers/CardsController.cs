using Microsoft.AspNetCore.Mvc;

namespace BananasGamblerBackend.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class CardsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
