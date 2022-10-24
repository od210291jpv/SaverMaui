using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SaverBackend.DTO;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetCategoriesController : ControllerBase
    {

        private ApplicationContext db;

        public GetCategoriesController(ApplicationContext database)
        {
            this.db = database;
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
                    AmountOfFavorites = ct.AmountOfFavorites,
                    AmountOfOpenings = ct.AmountOfOpenings,
                    PublisherProfileId = profileId,
                });
            }

            return results.ToArray();
        }

        [HttpPost(Name = "GetPopularCategories")]
        public async Task<CategoryDto[]> GetMostPopularCategories(int limit)
        {
            Category[] result = this.db.Categories.OrderByDescending(ct => ct.AmountOfOpenings).ToArray();

            CategoryDto[] limitedResult = new CategoryDto[limit];

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

        private async Task<Guid?> GetPublisherProfile(Guid categoryId) 
        {
            Profile? result = await this.db
                .Profiles
                .SingleOrDefaultAsync(pr =>
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
