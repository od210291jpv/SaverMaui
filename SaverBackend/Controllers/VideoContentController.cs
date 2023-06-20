using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SaverBackend.DTO;
using SaverBackend.Models;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoContentController : ControllerBase
    {
        private ApplicationContext db;

        public VideoContentController(ApplicationContext appContext)
        {
            this.db = appContext;
        }

        [HttpPost(Name = "AddVideo")]
        public async Task<int[]> AddVideo(SyncVideoDto payload) 
        {
            Profile? user = await this.db.Profiles.SingleOrDefaultAsync(p => p.ProfileId == payload.PublisherId);
            if (user == null)
            {
                return new int[1] { 0 } ;
            }

            foreach (var video in payload.VideoContent) 
            {
                if (this.db.Videos.Where(v => v.VideoUri == video.ImageUri).Count() == 0) 
                {
                    var res = this.db.Videos.Add(new VideoContent()
                    {
                        CategoryId = video.CategoryId,
                        Title = video.Title,
                        DateCreated = DateTime.UtcNow,
                        VideoUri = video.ImageUri,
                        ProfileId = user.Id
                    });

                    user.VideoContents.Add(res.Entity);
                }
            }

            await this.db.SaveChangesAsync();
            return this.db.Videos.Where(v => v.ProfileId == user.Id).Select(v => v.Id).ToArray();
        }

        [HttpGet(Name = "GetVideo")]
        public async Task<AddVideoDto[]> GetUserVideo(Guid userId) 
        {
            Profile? user = await this.db.Profiles.SingleOrDefaultAsync(p => p.ProfileId == userId);
            if (user == null)
            {
                return Array.Empty<AddVideoDto>();
            }

            return await this.db.Videos.Where(v => v.ProfileId == user.Id)
                .Select(c => new AddVideoDto() 
                {
                    CategoryId = c.CategoryId,
                    Title = c.Title,
                    DateCreated = c.DateCreated,
                    ImageUri=c.VideoUri,
                    PublisherProfileId = userId
                }).ToArrayAsync();
        }
    }
}
