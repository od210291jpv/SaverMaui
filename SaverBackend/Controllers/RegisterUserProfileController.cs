using Microsoft.AspNetCore.Mvc;

using SaverBackend.DTO;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterUserProfileController : ControllerBase
    {
        private ApplicationContext dbContext;

        public RegisterUserProfileController(ApplicationContext context)
        {
            this.dbContext = context;
        }

        [HttpPost("RegisterUserProfile")]
        public async Task<IActionResult> Index(UserProfileDto profileDto)
        {
            if (this.dbContext.Profiles.Select(pr => pr.UserName).ToArray().Contains(profileDto.UserName)) 
            {
                return BadRequest($"Username {profileDto.UserName} already exsists!");
            }

            Profile profile = new Profile()
            {
                UserName = profileDto.UserName,
                FavoriteCategories = new List<Category>(),
                Friends = new List<Profile>(),
                Groups = new List<Group>(),
                Password = profileDto.Password,
                ProfileId = Guid.NewGuid(),
                Publications = new List<Content>(),
                PublishedCategories = new List<Category>(),
                VerificationCode = profileDto.VerificationCode,
                IsOnline = false
            };

            this.dbContext.Profiles.Add(profile);

            await this.dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
