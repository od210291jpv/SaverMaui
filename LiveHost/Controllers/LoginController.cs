using LiveHost.DataBase;
using LiveHost.Dto;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace LiveHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private ApplicationContext dbContext;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;

        public LoginController(ApplicationContext appContext)
        {
            this.dbContext = appContext;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// loigin, get from config
            this.redisDb = redis.GetDatabase();
        }

        [HttpPost("Login")]
        public async Task<UserProfileInfoDto> Index(string login, string password)
        {
            if (this.dbContext.Profiles.Where(pr => pr.UserName == login && pr.Password == password).ToArray().Length != 1)
            {
                return new UserProfileInfoDto() { Error = "Unauthorized" };
            }

            DataBase.Models.Profile userProfile = this.dbContext.Profiles.Single(pr => pr.UserName == login && pr.Password == password);

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
            this.redisDb.KeyExpire(userProfile.UserName, TimeSpan.FromSeconds(30));
            return profileInfo;
        }
    }
}
