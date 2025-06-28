
using HtmlAgilityPack;
using RestSharp;

namespace ContentParserBackend.Services
{
    public class PediaSearchEngine : ISerachEngine
    {
        private string url;
        private RestClient restClient;

        public PediaSearchEngine(string url)
        {
            this.url = url;
            this.restClient = new RestClient();
        }

        public async Task<List<string>> ParseAsync(string keyword)
        {
            List<string> results = new List<string>();
            var response = await this.restClient.ExecuteGetAsync<string>(new RestRequest(this.url, method: Method.Get));


            var responseContent = response.Content;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseContent);

            // all "a" starting models for example
            var pages = document.DocumentNode.SelectNodes("//div[@class = 'shrt']/a").AsParallel().Select(a => a.GetAttributeValue("href", "")).Where(e => e != "").ToArray();

            // pagination for specific model miniatures set
            int pagination = 0;

            if (pages is not null && pages.Length > 0) 
            {
                foreach (var page in pages) 
                {
                    HtmlDocument pageWithContentMiniatures = new HtmlDocument();
                    var loadedPage = await this.restClient.ExecuteGetAsync<string>(new RestRequest(page, Method.Get));
                    pageWithContentMiniatures.LoadHtml(loadedPage.Content ?? "");

                    // check pagination - need to fix
                    var pag = pageWithContentMiniatures.DocumentNode.SelectSingleNode("//div[@class = 'nv-blk']//li[2]/span")?.InnerText?.Split("of")?.LastOrDefault()?.Replace(" ", "");
                    pagination = int.Parse(pag ?? "1");

                    for (int i = 1; i <= pagination; i++) 
                    {
                        var pageWithThumbnails = new HtmlDocument();
                        var url = page == "1" ? page + @$"{i}/" : page;
                        var html = await restClient.ExecuteGetAsync<string>(new RestRequest(url, method: Method.Get));

                        pageWithThumbnails.LoadHtml(html.Content ?? "");
                        // get all thumbnais urls and replace t_ part with ""


                        var res = pageWithThumbnails.DocumentNode.SelectNodes("//div[@class = 'shrt']/a").AsParallel().Select(i => i.GetAttributeValue("href", "")).Where(l => l.ToLower().Contains(keyword.ToLower()) == true);
                        results.AddRange(res);
                    }
                }

            }

            return results;
        }
    }
}
