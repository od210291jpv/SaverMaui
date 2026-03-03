using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
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
        private IDatabase redisContentDb;
        //private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public LoginController(ApplicationContext context, IHubContext<MainNotificationsHub> hubcontext)
        {
            this.dbContext = context;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase();       
            this.redisContentDb = redis.GetDatabase(1);       
            this.hub = hubcontext;
        }

        [HttpGet("ProfileInfo")]
        public async Task<UserProfileInfoDto> GetProfileInfo(string login, string password) 
        {
            //await this.webLogger.LogAsync($"Fetching profile info for user {login}", LogSeverity.Verbose);
            if (this.dbContext.Profiles.Where(pr => pr.UserName == login && pr.Password == password).ToArray().Length != 1)
            {
                //await this.webLogger.LogAsync($"No profile found for user {login} with provided credentials", LogSeverity.Warn);
                return new UserProfileInfoDto();
            }

            //await this.webLogger.LogAsync($"Profile found for user {login}, retrieving details", LogSeverity.Verbose);
            Profile userProfile = this.dbContext.Profiles.Single(pr => pr.UserName == login && pr.Password == password);

            //await this.webLogger.LogAsync($"Fetching published categories for user {login}", LogSeverity.Verbose);
            UserProfileInfoDto profileInfo = new UserProfileInfoDto()
            {
                UserName = userProfile.UserName,
                FavoriteCategories = userProfile.FavoriteCategories,
                Friends = userProfile.Friends,
                Groups = userProfile.Groups,
                IsOnline = true,
                ProfileId = userProfile.ProfileId,
                Publications = userProfile.Publications,
                Funds = userProfile.Funds,
                Id = userProfile.Id,
            };

            //await this.webLogger.LogAsync($"Mapping published categories for user {login}", LogSeverity.Verbose);
            return profileInfo;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Index(string login, string password)
        {
            //await this.webLogger.LogAsync($"Attempting login for user {login}", LogSeverity.Verbose);
            if (this.dbContext.Profiles.Where(pr => pr.UserName == login && pr.Password == password).ToArray().Length != 1) 
            {
               // await this.webLogger.LogAsync($"Login failed for user {login}: Invalid credentials", LogSeverity.Warn);
                return Unauthorized();
            }

            //await this.webLogger.LogAsync($"Login successful for user {login}, retrieving profile", LogSeverity.Verbose);
            Profile userProfile = this.dbContext.Profiles.Single(pr => pr.UserName ==  login && pr.Password == password);

            //await this.webLogger.LogAsync($"Fetching published categories for user {login}", LogSeverity.Verbose);
            var puCats = this.dbContext.Categories.Where(ct => ct.ProfileId == userProfile.Id).ToList();
            //await this.webLogger.LogAsync($"Total published categories found for user {login}: {puCats.Count}", LogSeverity.Verbose);

            var publishedCategoriesDto = puCats.Select(c => new CategoryDto() 
            {
                AmountOfFavorites = c.AmountOfFavorites,
                AmountOfOpenings = c.AmountOfOpenings,
                CategoryId = c.CategoryId,
                Name = c.Name,
                PublisherProfileId = c.Profile?.ProfileId,                
            }).ToList();

            //await this.webLogger.LogAsync($"Mapping published categories completed for user {login}", LogSeverity.Verbose);
            UserProfileInfoDto profileInfo = new UserProfileInfoDto()
            {
                UserName = userProfile.UserName,
                FavoriteCategories = userProfile.FavoriteCategories,
                Friends = userProfile.Friends,
                Groups = userProfile.Groups,
                IsOnline = true,
                ProfileId = userProfile.ProfileId,
                Publications = userProfile.Publications,
                PublishedCategories = publishedCategoriesDto,
                Funds = userProfile.Funds,
            };

            //await this.webLogger.LogAsync($"Setting user {login} status to Online in Redis", LogSeverity.Verbose);
            this.redisDb.StringSet(userProfile.UserName, "Online");
            this.redisDb.KeyExpire(userProfile.UserName, TimeSpan.FromMinutes(30));
            //await this.webLogger.LogAsync($"User {login} status set to Online in Redis", LogSeverity.Verbose);
            await this.LoadContentToRedis();

            await this.hub.Clients.All.SendAsync("UserLoggedIn", userProfile.UserName);
            return Ok();
        }

        [HttpGet("/SyncRedis")]
        public async Task SyncRedis() 
        {
            //await this.webLogger.LogAsync("Manual trigger to sync content to Redis", LogSeverity.Verbose);
            await LoadContentToRedis();
            //await this.webLogger.LogAsync("Content sync to Redis completed", LogSeverity.Verbose);
        }

        private async Task LoadContentToRedis() 
        {
            //await this.webLogger.LogAsync("Checking if content is already synced to Redis", LogSeverity.Verbose);
            var isSynced = await this.redisDb.StringGetAsync("content_synced");

            //await this.webLogger.LogAsync($"Content sync status: {isSynced}", LogSeverity.Verbose);
            if (isSynced == "true")
            {
                //await this.webLogger.LogAsync("Content is already synced to Redis, skipping sync", LogSeverity.Verbose);
                return;
            }

            //await this.webLogger.LogAsync("Starting content sync to Redis", LogSeverity.Verbose);
            var allContent = this.dbContext.Contents.ToArray();

            //await this.webLogger.LogAsync($"Total content items to sync: {allContent.Length}", LogSeverity.Verbose);
            foreach (var content in allContent) 
            {
                await this.redisContentDb.StringSetAsync(content.Id.ToString(), JsonConvert.SerializeObject(content));
            }

            //await this.webLogger.LogAsync("Content sync to Redis completed", LogSeverity.Verbose);
            this.redisDb.StringSet("content_synced", "true");
            //await this.webLogger.LogAsync("Set content_synced flag in Redis to true", LogSeverity.Verbose);
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
        public async Task<bool> GetLoginState(string login, string password) 
        {
            //await this.webLogger.LogAsync($"Checking login status for user {login}", LogSeverity.Verbose);
            var credsAreCorrect = this.dbContext.Profiles.Where(pr => pr.UserName == login && pr.Password == password).ToArray().Length == 1;
            var loginData = await this.redisDb.StringGetAsync(login);

            //await this.webLogger.LogAsync($"Credentials valid: {credsAreCorrect}, Redis login data: {loginData}", LogSeverity.Verbose);
            if (credsAreCorrect == false) 
            {
                //await this.webLogger.LogAsync($"Invalid credentials for user {login}", LogSeverity.Warn);
                return false;
            }

            if (loginData.HasValue == false) 
            {
                //await this.webLogger.LogAsync($"No login data found in Redis for user {login}", LogSeverity.Warn);
                return false;
            }

            if (await this.redisDb.StringGetAsync(login) == "Offline") 
            {
                //await this.webLogger.LogAsync($"User {login} is marked as Offline in Redis", LogSeverity.Warn);
                return false;
            }

            //await this.webLogger.LogAsync($"User {login} is currently Online", LogSeverity.Verbose);
            return true;
        }
    }
}
