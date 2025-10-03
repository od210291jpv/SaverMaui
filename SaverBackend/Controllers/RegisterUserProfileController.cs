using Microsoft.AspNetCore.Mvc;

using SaverBackend.DTO;
using SaverBackend.Models;
using WebLoggerClient;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterUserProfileController : ControllerBase
    {
        private ApplicationContext dbContext;
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public RegisterUserProfileController(ApplicationContext context)
        {
            this.dbContext = context;
        }

        [HttpPost("RegisterUserProfile")]
        public async Task<IActionResult> Index(UserProfileDto profileDto)
        {
            await this.webLogger.LogAsync($"Attempting to register user with username: {profileDto.UserName}", LogSeverity.Verbose);
            if (this.dbContext.Profiles.Select(pr => pr.UserName).ToArray().Contains(profileDto.UserName)) 
            {
                await this.webLogger.LogAsync($"Username {profileDto.UserName} already exists!", LogSeverity.Warn);
                return BadRequest($"Username {profileDto.UserName} already exists!");
            }

            await this.webLogger.LogAsync($"Username {profileDto.UserName} is available. Proceeding with registration.", LogSeverity.Verbose);
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

            await this.webLogger.LogAsync($"Adding new user profile for {profileDto.UserName} to the database", LogSeverity.Verbose);
            this.dbContext.Profiles.Add(profile);

            await this.dbContext.SaveChangesAsync();

            await this.webLogger.LogAsync($"User profile for {profileDto.UserName} registered successfully", LogSeverity.Verbose);
            return Ok();
        }
    }
}
