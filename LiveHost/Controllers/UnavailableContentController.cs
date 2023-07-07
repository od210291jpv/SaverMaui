using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using LiveHost.DataStructures;
using LiveHost.DataBase.Models;
using LiveHost.DataBase;

namespace LiveHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnavailableContentController : ControllerBase
    {
        private readonly ApplicationContext context;

        public UnavailableContentController(ApplicationContext context)
        {
            this.context = context;
        }

        [HttpGet(Name = "GetUnavailableContent")]
        public async Task<List<ContentAvailabilityInfo>> Index()
        {
            Content[] allContent = await this.context.Contents.ToArrayAsync();
            List<ContentAvailabilityInfo> unavailableContent = new List<ContentAvailabilityInfo>();

            RestClient client = new RestClient();

            foreach (var content in allContent) 
            {
                if (content.ImageUri != null && content.ImageUri != "") 
                {
                    var result = await client.ExecuteGetAsync(new RestRequest(content.ImageUri, Method.Get));

                    if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        unavailableContent.Add(new ContentAvailabilityInfo()
                        {
                            Url = content.ImageUri,
                            ContentTitle = content.Title,
                            StatusCode = result.StatusCode,
                            CategoryName = this.context.Categories.SingleOrDefault(c => c.CategoryId == content.CategoryId)?.Name ?? "Undefined"
                        });

                        System.Console.WriteLine($"Orphan content found, {content.Title}");
                    }
                }
            }

            return unavailableContent;
        }
    }
}
