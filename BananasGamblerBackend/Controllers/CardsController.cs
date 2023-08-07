using Microsoft.AspNetCore.Mvc;

namespace BananasGamblerBackend.Controllers
{
    public class CardsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
