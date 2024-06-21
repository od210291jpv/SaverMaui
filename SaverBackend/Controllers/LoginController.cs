using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SaverBackend.DTO;
using SaverBackend.Hubs;
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
        private IHubContext<MainNotificationsHub> hub;
        private IDatabase redisDb;

        public LoginController(ApplicationContext context, IHubContext<MainNotificationsHub> hubcontext)
        {
            this.dbContext = context;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase();       
            this.hub = hubcontext;
        }

        [HttpPost("Login")]
        public async Task<UserProfileInfoDto> Index(string login, string password)
        {
            if (this.dbContext.Profiles.Where(pr => pr.UserName == login && pr.Password == password).ToArray().Length != 1) 
            {
                return new UserProfileInfoDto() { Error = "Unauthorized" };
            }

            Profile userProfile = this.dbContext.Profiles.Single(pr => pr.UserName ==  login && pr.Password == password);

            var puCats = this.dbContext.Categories.Where(ct => ct.ProfileId == userProfile.Id).ToList();

            var publishedCategoriesDto = puCats.Select(c => new CategoryDto() 
            {
                AmountOfFavorites = c.AmountOfFavorites,
                AmountOfOpenings = c.AmountOfOpenings,
                CategoryId = c.CategoryId,
                Name = c.Name,
                PublisherProfileId = c.Profile?.ProfileId,                
            }).ToList();

            UserProfileInfoDto profileInfo = new UserProfileInfoDto()
            {
                UserName = userProfile.UserName,
                FavoriteCategories = userProfile.FavoriteCategories,
                Friends = userProfile.Friends,
                Groups = userProfile.Groups,
                IsOnline = true,
                ProfileId = userProfile.ProfileId,
                Publications = userProfile.Publications,
                PublishedCategories = publishedCategoriesDto
            };
            
            this.redisDb.StringSet(userProfile.UserName, "Online");
            this.redisDb.KeyExpire(userProfile.UserName, TimeSpan.FromMinutes(30));
            await this.hub.Clients.All.SendAsync($"{profileInfo.UserName} just joined!! Say Hello!");
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
