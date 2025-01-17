using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaverBackend.Models;
using SaverBackend.Services.RabbitMq;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController : Controller
    {
        private ApplicationContext db;
        private IRabbitMqService mqService;

        public ContentController(ApplicationContext db, IRabbitMqService mq)
        {
            this.db = db;
            this.mqService = mq;
        }

        [HttpGet("UpdateContentRating")]
        public async Task<IActionResult> Index(int contentId, short rating, int? profileId)
        {
            var content = this.db.Contents.SingleOrDefault(c => c.Id == contentId);
            if (content is null) 
            {
                return NotFound($"No content with the ID {contentId}");
            }

            content.Rating = rating;

            if (profileId is not null) 
            {
                var user = this.db.Profiles.SingleOrDefault(c => c.Id == profileId);
                if (user is null) 
                {
                    return NotFound($"Profile with id {profileId} not found");
                }

                user.Funds += 30.0m;
            }

            await this.db.SaveChangesAsync();
            return Ok(content);
        }

        [HttpGet("initContentCost")]
        public IActionResult InitContentCost() 
        {
            this.mqService.SendMessage("init", "InitContentQueue");
            return Ok();
        }

        [HttpGet("BuyContent")]
        public async Task<IActionResult> PurcahseContent(int userId, int contentId)
        {
            var content = this.db.Contents.SingleOrDefault<Content>(c => c.Id == contentId);
            if (content is null)
            {
                return NotFound($"No content with the ID {contentId}");
            }

            var user = this.db.Profiles.Single(p => p.Id == userId);
            if (user is null)
            {
                return NotFound($"No user with the ID {contentId}");
            }

            if (user.Funds < content.Cost) 
            {
                return BadRequest("The user has no enough funds");
            }

            user.Funds = user.Funds - content.Cost;

            if (content != null)
            {
                if (await this.db.FavoriteContent.SingleOrDefaultAsync(f => f.FavoriteContentId == content.Id &&
                f.ProfileId == user.Id) is null)
                {
                    user.FavoriteContent.Add(content);
                }
            }

            await this.db.SaveChangesAsync();
            return Ok();
        }
    }
}
