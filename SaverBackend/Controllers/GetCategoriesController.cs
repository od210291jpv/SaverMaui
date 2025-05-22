using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaverBackend.DTO;
using SaverBackend.Models;
using StackExchange.Redis;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetCategoriesController : ControllerBase
    {

        private ApplicationContext db;

        private ConnectionMultiplexer redis;
        private IDatabase redisDb;
        private IDatabase redisContentDb;

        public GetCategoriesController(ApplicationContext database)
        {
            this.db = database;

            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase();
            this.redisContentDb = redis.GetDatabase(1);
        }

        [HttpGet(Name = "GetCategories")]
        public async Task<CategoryDto[]> Index()
        {
            Category[]? allCategories = await db.Categories.ToArrayAsync();
            List<CategoryDto> results = new();

            foreach (var ct in allCategories) 
            {
                Guid? profileId = await GetPublisherProfile(ct.CategoryId);
                results.Add(new CategoryDto()
                {
                    Name = ct.Name,
                    CategoryId = ct.CategoryId,
                    Id = ct.Id,
                    AmountOfFavorites = ct.AmountOfFavorites,
                    AmountOfOpenings = ct.AmountOfOpenings,
                    PublisherProfileId = profileId,
                });
            }

            return results.ToArray();
        }

        [HttpGet("categoryContent")]
        public async Task<ContentDto[]> GetCategoryContent(Guid categoryId) 
        {
            var keys = await this.db.Contents.Where(c => c.CategoryId == categoryId).Select(c => c.Id).ToArrayAsync();
            var result = keys.Select(k => JsonConvert.DeserializeObject<ContentDto>(this.redisContentDb.StringGet(k.ToString()))).ToArray();
            return result;
        }

        [HttpPost(Name = "GetPopularCategories")]
        public async Task<CategoryDto[]> GetMostPopularCategories(int limit)
        {
            Category[] result = await this.db.Categories.OrderByDescending(ct => ct.AmountOfOpenings).ToArrayAsync();
            CategoryDto[] limitedResult = new CategoryDto[limit];


            if (result.Length < limit) 
            {
                return limitedResult;
            }

            for (int i = 0; i != limit; i++)
            {
                Guid? profileId = await GetPublisherProfile(result[i].CategoryId);

                limitedResult[i] = new CategoryDto()
                {
                    Name = result[i].Name,
                    CategoryId = result[i].CategoryId,
                    AmountOfFavorites = result[i].AmountOfFavorites,
                    AmountOfOpenings = result[i].AmountOfOpenings,
                    PublisherProfileId = profileId
                };
            }
            return limitedResult;
        }

        [HttpGet("GetMostFavoriteCategories")]
        public async Task<CategoryDto[]> GetMostFavoriteCategories(int categoriesLimit)
        {
            Category[] allCategories = await this.db.Categories.OrderByDescending(ct => ct.AmountOfFavorites).ToArrayAsync();
            CategoryDto[] limitedResult = new CategoryDto[categoriesLimit];

            if (allCategories.Length < categoriesLimit)
            {
                return limitedResult;
            }

            for (int i = 0; i != categoriesLimit; i++)
            {
                Guid? profileId = await GetPublisherProfile(allCategories[i].CategoryId);

                limitedResult[i] = new CategoryDto()
                {
                    Name = allCategories[i].Name,
                    CategoryId = allCategories[i].CategoryId,
                    AmountOfFavorites = allCategories[i].AmountOfFavorites,
                    AmountOfOpenings = allCategories[i].AmountOfOpenings,
                    PublisherProfileId = profileId
                };
            }

            return limitedResult;
        }

        private async Task<Guid?> GetPublisherProfile(Guid categoryId) 
        {
            Profile? result = await this.db
                .Profiles
                .FirstOrDefaultAsync(pr =>
                pr.PublishedCategories != null &&
                pr.PublishedCategories.Select(c => c.CategoryId)
                .ToArray()
                .Contains(categoryId));

            if (result is null) 
            {
                return null;
            }

            return result.ProfileId;
        }
    }
}
