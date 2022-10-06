using Microsoft.AspNetCore.Mvc;

using SaverBackend.DTO;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private ApplicationContext dbContext;

        public LoginController(ApplicationContext context)
        {
            this.dbContext = context;
        }

        [HttpPost("Login")]
        public async Task<UserProfileInfoDto> Index(string login, string password)
        {
            if (this.dbContext.Profiles.Where(pr => pr.UserName == login && pr.Password == password).ToArray().Length != 1) 
            {
                return new UserProfileInfoDto() { Error = "Unauthorized" };
            }

            var userProfile = this.dbContext.Profiles.Single(pr => pr.UserName ==  login && pr.Password == password);

            UserProfileInfoDto profileInfo = new UserProfileInfoDto()
            {
                UserName = userProfile.UserName,
                FavoriteCategories = userProfile.FavoriteCategories,
                Friends = userProfile.Friends,
                Groups = userProfile.Groups,
                IsOnline = true,
                ProfileId = userProfile.ProfileId,
                Publications = userProfile.Publications,
                PublishedCategories = userProfile.PublishedCategories
            };

            userProfile.IsOnline = true;

            await this.dbContext.SaveChangesAsync();

            return profileInfo;
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(string login, string password) 
        {
            if (this.dbContext.Profiles.Select(pr => pr.UserName == login && pr.Password == password).ToArray().Length != 1)
            {
                return Unauthorized();
            }

            var userProfile = this.dbContext.Profiles.Single(pr => pr.UserName == login && pr.Password == password);

            userProfile.IsOnline = false;

            await this.dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
