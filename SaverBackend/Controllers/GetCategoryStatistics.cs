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
        //private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public GetCategoryStatistics(ApplicationContext database)
        {
            this.db = database;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");
            this.redisDb = redis.GetDatabase();
        }

        [HttpGet(Name = "GetStatistics")]
        public async Task<StatisticsDto> Index()
        {
            //await this.webLogger.LogAsync("Getting category statistics from database", LogSeverity.Verbose);
            StatisticsDto statisticsData = new();

            //await this.webLogger.LogAsync("Iterating through categories to compile statistics", LogSeverity.Verbose);
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

            //await this.webLogger.LogAsync($"Total categories processed for statistics: {statisticsData.Items.Count}", LogSeverity.Verbose);
            return statisticsData;
        }
    }
}
