using Microsoft.AspNetCore.Mvc;

using SaverBackend.DTO;
using SaverBackend.Models;
using StackExchange.Redis;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetCategoryStatistics : Controller
    {
        private ApplicationContext db;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;

        public GetCategoryStatistics(ApplicationContext database)
        {
            this.db = database;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");
            this.redisDb = redis.GetDatabase();
        }

        [HttpGet(Name = "GetStatistics")]
        public async Task<StatisticsDto> Index()
        {

            StatisticsDto statisticsData = new();

            foreach (var cat in this.db.Categories) 
            {
                statisticsData.Items
                    .Add(new StatisticsItem() 
                    {
                        CategoryId = cat.CategoryId,
                        Favorites = cat.AmountOfFavorites,
                        Openings = cat.AmountOfOpenings
                    });

            }

            return statisticsData;
        }
    }
}
