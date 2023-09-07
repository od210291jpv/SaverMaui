using BananasGamblerBackend.Database;
using BananasGamblerBackend.Database.Models;
using BananasGamblerBackend.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BananasGamblerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private ApplicationContext context;

        public LoginController(ApplicationContext context)
        {
            this.context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto model) 
        {
            if (ModelState.IsValid) 
            {
                User? user = await this.context.Users.FirstOrDefaultAsync(u => model.Login == u.Login && model.Password == u.Password);
                if (user != null) 
                {
                    await Authenticate(model.Login);
                    return Ok();
                }
                ModelState.AddModelError("", "Incorrect credentials");
                return Unauthorized();
            }
            return Unauthorized();
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
