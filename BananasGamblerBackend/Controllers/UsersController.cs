using Microsoft.AspNetCore.Mvc;

namespace BananasGamblerBackend.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
