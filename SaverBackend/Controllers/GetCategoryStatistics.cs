using Microsoft.AspNetCore.Mvc;
using SaverBackend.DTO;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetCategoryStatistics : Controller
    {
        private ApplicationContext db;

        public GetCategoryStatistics(ApplicationContext database)
        {
            this.db = database;
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
