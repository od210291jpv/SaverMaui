using HtmlAgilityPack;
using RestSharp;
using System;

namespace ContentParserBackend.Services
{
    public class SerachEngine
    {
        private List<string> results;
        private Uri baseUrl;
        private RestClient restClient;
        public int PagesAmount = 0;

        public SerachEngine(string url)
        {
            this.results = new List<string>();
            this.baseUrl = new Uri(url);
            this.restClient = new RestClient();
        }

        public async Task<List<string>> ParseAsync(string keyword)
        {
            this.results.Clear();

            RestRequest request = new RestRequest(this.baseUrl, Method.Get);
            var response = await this.restClient.ExecuteGetAsync<string>(request);

            var responseContent = response.Content;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseContent);

            var pages = document.DocumentNode.SelectNodes("//div[@class = 'navipageto'][2]//li").ToArray();
            this.PagesAmount = pages.Length;

            foreach (var page in pages)
            {
                var cont = new HtmlDocument();
                cont.LoadHtml(page.InnerHtml);
                var node = cont.DocumentNode.SelectSingleNode("/a");
                var pageLink = node?.GetAttributeValue("href", "") ?? "";

                var responce = await this.restClient.ExecuteGetAsync<string>(new RestRequest(new Uri(pageLink), Method.Get));
                var pageLinkContent = responce.Content ?? "";

                var pageWithContent = new HtmlDocument();
                pageWithContent.LoadHtml(pageLinkContent);

                var pageContent = pageWithContent.DocumentNode.SelectNodes("//div[@class = 'previzako']");

                if (pageContent != null) 
                {
                    foreach (var cnt in pageContent)
                    {
                        var cc = new HtmlDocument();
                        cc.LoadHtml(cnt.InnerHtml);

                        var link = cc.DocumentNode.SelectSingleNode("/a")?.GetAttributeValue("href", "");

                        if (link != null && link.ToLower().Contains(keyword.ToLower()))
                        {
                            this.results.Add(link);
                        }
                    }
                }
            }

            return results;
        }
    }
}
