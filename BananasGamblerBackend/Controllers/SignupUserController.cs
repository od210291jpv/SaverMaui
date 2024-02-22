using BananasGamblerBackend.Database;
using BananasGamblerBackend.Database.Models;
using BananasGamblerBackend.Dto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BananasGamblerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignupUserController : ControllerBase
    {
        private ApplicationContext context;

        public SignupUserController(ApplicationContext context)
        {
            this.context = context;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var user = this.context.Users.FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);
                if (user is null)
                {
                    await this.context.Users.AddAsync(new User() { Login = model.Login, Password = model.Password });
                    await this.context.SaveChangesAsync();
                    await Authenticate(model.Login);
                    return Ok(user);
                }

                return BadRequest();
            }

            return BadRequest("User already registered");
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
