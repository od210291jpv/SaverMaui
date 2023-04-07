using HtmlAgilityPack;
using RestSharp;

namespace SaverMaui.Services.FeedParser
{
    internal class FeedParserService
    {
        private RestClient client;
        private string htmlDocument;
        private Uri feedUrl;

        private const string ImageXpath = "//figure//img";
        private const string TitleXpath = "//header[@class = 'tl_article_header']/h1";

        public FeedParserService(Uri feedUrl)
        {
            this.feedUrl = feedUrl;
            this.client = new RestClient();
        }

        public async Task LoadFeedByUrl() 
        {
            var response = await this.client.ExecuteGetAsync(new RestRequest(this.feedUrl, Method.Get));
            if(response?.Content is not null) 
            {
                this.htmlDocument = response.Content;
            }
        }

        public string GetFeedTitle() 
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(this.htmlDocument);

            string title = "placeholder";

            title = htmlDocument.DocumentNode.SelectSingleNode(TitleXpath).GetDirectInnerText();
            return title;
        }

        public List<string> GetFeedImages() 
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(this.htmlDocument);
            var images = htmlDocument.DocumentNode.SelectNodes(ImageXpath);
            var processedImages = images.Select(i => i.GetAttributeValue("src", "")).ToArray();

            var postprocessedImages = Array.Empty<string>;

            

            List<string> results = new List<string>();

            foreach(var item in processedImages) 
            {                
                if(item != "") 
                {
                    if (!item.Contains("https://telegra.ph") && !item.Contains("teletype"))
                    {
                        var fixedImg = $"https://telegra.ph{item}";
                        results.Add(fixedImg);
                    }
                    else 
                    {
                        results.Add(item);
                    }
                }
            }

            return results;
        }
    }
}
