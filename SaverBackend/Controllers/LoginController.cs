using Microsoft.AspNetCore.Mvc;

using SaverBackend.DTO;
using SaverBackend.Models;

using StackExchange.Redis;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private ApplicationContext dbContext;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;

        public LoginController(ApplicationContext context)
        {
            this.dbContext = context;
            this.redis = ConnectionMultiplexer.Connect("192.168.0.101:6379");
            this.redisDb = redis.GetDatabase();            
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
            
            this.redisDb.StringSet(userProfile.UserName, "Online");
            this.redisDb.KeyExpire(userProfile.UserName, TimeSpan.FromSeconds(30));
            return profileInfo;
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(string login, string password) 
        {
            var result = this.dbContext.Profiles.Where(pr => pr.UserName == login && pr.Password == password).ToArray().Length;

            if (this.dbContext.Profiles.Where(pr => pr.UserName == login && pr.Password == password).ToArray().Length != 1)
            {
                return Unauthorized();
            }

            var userProfile = this.dbContext.Profiles.Single(pr => pr.UserName == login && pr.Password == password);

            this.redisDb.StringSet(userProfile.UserName, "Offline");
            this.redisDb.KeyExpire(userProfile.UserName, TimeSpan.FromSeconds(30));
            return Ok();
        }

        [HttpGet("GetLoginStatus")]
        public async Task<bool> GetLoginState(string login) 
        {
            var loginData = await this.redisDb.StringGetAsync(login);

            if (loginData.HasValue == false) 
            {
                return false;
            }

            if (await this.redisDb.StringGetAsync(login) == "Offline") 
            {
                return false;
            }

            return true;
        }
    }
}
