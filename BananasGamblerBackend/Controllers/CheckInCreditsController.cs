using BananasGamblerBackend.Database;
using BananasGamblerBackend.Database.Models;
using BananasGamblerBackend.Dto;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BananasGamblerBackend.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CheckInCreditsController : ControllerBase
    {
        private ApplicationContext context;

        public CheckInCreditsController(ApplicationContext db)
        {
            this.context = db;
        }

        [HttpPost("checkin")]        
        public async Task<IActionResult> Index(LoginDto loginData, string code)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.Login == loginData.Login && u.Password == loginData.Password);
            if (user != null)
            {
                var checkinCode = await this.context.CheckInCodes.FirstOrDefaultAsync(c => c.Code == code);
                if (checkinCode == null || checkinCode.IsActive == false) 
                {
                    return NotFound("Checkin code was not found");
                }

                user.Credits += checkinCode.Amount;
                checkinCode.IsActive = false;

                await this.context.SaveChangesAsync();

                return Ok(user);
            }

            return Unauthorized();
        }

        [HttpGet("initCodes")]
        public async Task<IActionResult> InintNewCodes(int amount) 
        {
            for (int i = 0; i < amount; i++) 
            {
                this.context.CheckInCodes.Add(new CheckInCode()
                {
                    Code = $"{new Faker().Vehicle.Model()}{new Faker().Company.CompanyName()}",
                    Amount  = 100,
                    IsActive = true
                }) ;

                await this.context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
