using LiveHost.Dto;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SaverBackend.Models;

namespace LiveHost.Hubs
{
    public class MainFeedHub : Hub
    {
        private readonly ApplicationContext db;

        public MainFeedHub(ApplicationContext db)
        {
            this.db = db;
        }

        public async Task GetAllContentAsync()
        {
            List<ContentDto> content = new();

            var allContent = await this.db.Contents.ToArrayAsync();

            foreach (var c in allContent) 
            {
                content.Add(new ContentDto() 
                {
                    CategoryId = c.CategoryId,
                    DateCreated = c.DateCreated,
                    ImageUri = c.ImageUri,
                    Title = c.Title
                });
            }

            await this.Clients.All.SendAsync("GetAllContentAsync", content);
        }

        public async Task GetRandomContent() 
        {
            var allContent = await this.db.Contents.ToArrayAsync();
            await this.Clients.All.SendAsync("GetRandomContent", allContent[new Random().Next(0, allContent.Length - 1)]);
        }
    }
}
