using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaverBackend.DTO;
using SaverBackend.Models;
using StackExchange.Redis;
using WebLoggerClient;

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
        private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

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
            await this.webLogger.LogAsync("Getting all categories from database", LogSeverity.Verbose);
            Category[]? allCategories = await db.Categories.ToArrayAsync();
            await this.webLogger.LogAsync($"Total categories found: {allCategories.Length}", LogSeverity.Verbose);
            List<CategoryDto> results = new();

            await this.webLogger.LogAsync("Mapping categories to CategoryDto", LogSeverity.Verbose);
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

            await this.webLogger.LogAsync("Category mapping completed", LogSeverity.Verbose);
            return results.ToArray();
        }

        [HttpGet("categoryContent")]
        public async Task<ContentDto[]> GetCategoryContent(Guid categoryId) 
        {
            await this.webLogger.LogAsync($"Getting content for category with id {categoryId}", LogSeverity.Verbose);
            var keys = await this.db.Contents.Where(c => c.CategoryId == categoryId).Select(c => c.Id).ToArrayAsync();
            await this.webLogger.LogAsync($"Total content items found for category {categoryId}: {keys.Length}", LogSeverity.Verbose);
            var result = keys.Select(k => JsonConvert.DeserializeObject<ContentDto>(this.redisContentDb.StringGet(k.ToString()))).ToArray();
            await this.webLogger.LogAsync($"Content retrieval from Redis completed for category {categoryId}", LogSeverity.Verbose);
            return result;
        }

        [HttpPost(Name = "GetPopularCategories")]
        public async Task<CategoryDto[]> GetMostPopularCategories(int limit)
        {
            await this.webLogger.LogAsync($"Getting top {limit} most popular categories from database", LogSeverity.Verbose);
            Category[] result = await this.db.Categories.OrderByDescending(ct => ct.AmountOfOpenings).ToArrayAsync();
            await this.webLogger.LogAsync($"Total categories found: {result.Length}", LogSeverity.Verbose);
            CategoryDto[] limitedResult = new CategoryDto[limit];

            await this.webLogger.LogAsync("Mapping popular categories to CategoryDto", LogSeverity.Verbose);
            if (result.Length < limit) 
            {
                await this.webLogger.LogAsync($"Not enough categories to fulfill the limit of {limit}. Returning empty array.", LogSeverity.Warn);
                return limitedResult;
            }

            for (int i = 0; i != limit; i++)
            {
                await this.webLogger.LogAsync($"Processing category {i + 1} of {limit}", LogSeverity.Verbose);
                Guid? profileId = await GetPublisherProfile(result[i].CategoryId);

                await this.webLogger.LogAsync($"Category {i + 1}: {result[i].Name}, Openings: {result[i].AmountOfOpenings}, Favorites: {result[i].AmountOfFavorites}", LogSeverity.Verbose);
                limitedResult[i] = new CategoryDto()
                {
                    Name = result[i].Name,
                    CategoryId = result[i].CategoryId,
                    AmountOfFavorites = result[i].AmountOfFavorites,
                    AmountOfOpenings = result[i].AmountOfOpenings,
                    PublisherProfileId = profileId
                };
            }

            await this.webLogger.LogAsync("Popular category mapping completed", LogSeverity.Verbose);
            return limitedResult;
        }

        [HttpGet("GetMostFavoriteCategories")]
        public async Task<CategoryDto[]> GetMostFavoriteCategories(int categoriesLimit)
        {
            await this.webLogger.LogAsync($"Getting top {categoriesLimit} most favorited categories from database", LogSeverity.Verbose);
            Category[] allCategories = await this.db.Categories.OrderByDescending(ct => ct.AmountOfFavorites).ToArrayAsync();
            await this.webLogger.LogAsync($"Total categories found: {allCategories.Length}", LogSeverity.Verbose);
            CategoryDto[] limitedResult = new CategoryDto[categoriesLimit];

            await this.webLogger.LogAsync("Mapping most favorited categories to CategoryDto", LogSeverity.Verbose);
            if (allCategories.Length < categoriesLimit)
            {
                await this.webLogger.LogAsync($"Not enough categories to fulfill the limit of {categoriesLimit}. Returning empty array.", LogSeverity.Warn);
                return limitedResult;
            }

            await this.webLogger.LogAsync("Starting to process each category for mapping", LogSeverity.Verbose);
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

            await this.webLogger.LogAsync("Most favorited category mapping completed", LogSeverity.Verbose);
            return limitedResult;
        }

        private async Task<Guid?> GetPublisherProfile(Guid categoryId) 
        {
            await this.webLogger.LogAsync($"Searching for publisher profile for category id {categoryId}", LogSeverity.Verbose);
            Profile? result = await this.db
                .Profiles
                .FirstOrDefaultAsync(pr =>
                pr.PublishedCategories != null &&
                pr.PublishedCategories.Select(c => c.CategoryId)
                .ToArray()
                .Contains(categoryId));

            if (result is null) 
            {
                await this.webLogger.LogAsync($"No publisher profile found for category id {categoryId}", LogSeverity.Warn);
                return null;
            }

            await this.webLogger.LogAsync($"Publisher profile found for category id {categoryId}: ProfileId = {result.ProfileId}", LogSeverity.Verbose);
            return result.ProfileId;
        }
    }
}
