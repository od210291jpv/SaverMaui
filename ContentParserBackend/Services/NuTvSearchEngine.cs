
using ContentParserBackend.Dto;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;

namespace ContentParserBackend.Services
{
    public class NuTvSearchEngine : ISerachEngine
    {
        private List<string> results = new();
        private string url = string.Empty;
        private string startLetter;
        private RestClient restClient;

        public NuTvSearchEngine(string url, string startLetter)
        {
            this.url = url;
            this.restClient = new RestClient(this.url);
            this.startLetter = startLetter;
        }

        public async Task<List<string>> ParseAsync(string keyword, IRabbitMqService mqClient)
        {
            // open the start letter page
            List<string> intermediateLinks = new List<string>();
            this.results.Clear();
            SendLogMessage("Starting nutv engine", LogSeverity.Verbose, ref mqClient);
            RestRequest request = new RestRequest($"models/{this.startLetter}/1/", Method.Get);

            SendLogMessage($"Parsing content for {startLetter} start letter", LogSeverity.Verbose, ref mqClient);
            var response = await this.restClient.ExecuteGetAsync<string>(request);
            var responseContent = response.Content;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseContent);
            // get the pages amount
            var paginationHolder = document.DocumentNode.SelectNodes("//div[@class = 'pagination-holder']")[1];
            var amountOfPages = paginationHolder.SelectNodes("//ul/li").ToArray().Length;
            SendLogMessage($"Amount of pages for the letter {startLetter}: {amountOfPages}", LogSeverity.Verbose, ref mqClient);

            // parse preview links on each page and put in links collection
            SendLogMessage($"Parsing preview linkns for {startLetter} keyword {keyword}", LogSeverity.Verbose, ref mqClient);
            for (int i = 1; i <= amountOfPages; i++)
            {
                RestRequest pageRequest = new RestRequest($"models/{this.startLetter}/{i}/", Method.Get);
                var pageResponse = await this.restClient.ExecuteGetAsync<string>(pageRequest);
                var pageContent = pageResponse.Content;

                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContent);
                var imagesGrid = pageDocument.GetElementbyId("list_videos_related_videos_items");
                var previewNodes = imagesGrid?.SelectNodes("//div[@class = 'item']/a").ToArray();
                if (previewNodes != null)
                {
                    foreach (var node in previewNodes)
                    {
                        //var linkNode = node.SelectSingleNode("//a");
                        var link = node?.GetAttributeValue("href", "");
                        SendLogMessage($"Found preview link {link} checking agains {keyword} with start letter {startLetter}", LogSeverity.Verbose, ref mqClient);
                        if (link != null && link.ToLower().Contains(keyword.ToLower()))
                        {
                            SendLogMessage($"Found mathing link {link} with keyword {keyword}. Adding intermediate link", LogSeverity.Verbose, ref mqClient);
                            intermediateLinks.Add(link);
                        }
                    }
                }
            }

            SendLogMessage($"Starting parsing {intermediateLinks.Count()} intermediate links", LogSeverity.Verbose, ref mqClient);
            foreach (var link in intermediateLinks)
            {
                SendLogMessage($"Parsing intermediate link: {link}", LogSeverity.Verbose, ref mqClient);
                RestRequest contentRequest = new RestRequest(link, Method.Get);
                var contentResponse = await this.restClient.ExecuteGetAsync<string>(contentRequest);
                var contentPage = contentResponse.Content;
                HtmlDocument contentDocument = new HtmlDocument();
                contentDocument.LoadHtml(contentPage);

                SendLogMessage("Parsing thumbs", LogSeverity.Verbose, ref mqClient);
                this.ParseThumbs(contentDocument, keyword);
                SendLogMessage($"Current result amount {this.results.Count()}", LogSeverity.Verbose, ref mqClient);

                var hasNext = contentDocument.DocumentNode.SelectSingleNode("//li[@class = 'next']") != null;
                SendLogMessage($"Intermediate link has next page, parsing..", LogSeverity.Verbose, ref mqClient);

                while (hasNext)
                {
                    var nextLink = contentDocument.DocumentNode.SelectSingleNode("//li[@class = 'next']/a")?.GetAttributeValue("href", "");
                    RestRequest nextContentRequest = new RestRequest(nextLink, Method.Get);
                    var nextContentResponse = await this.restClient.ExecuteGetAsync<string>(nextContentRequest);
                    var nextContentPage = nextContentResponse.Content;
                    contentDocument.LoadHtml(nextContentPage);

                    this.ParseThumbs(contentDocument, keyword);

                    hasNext = contentDocument.DocumentNode.SelectSingleNode("//li[@class = 'next']") != null;
                }
            }
            SendLogMessage($"Done parsing! Final results {results.Count()}", LogSeverity.Verbose, ref mqClient);
            return this.results;
        }

        private void ParseThumbs(HtmlDocument html, string keyword)
        {
            var thumbs = html.DocumentNode.SelectNodes("//img[@class = 'thumb']");
            if (thumbs != null)
            {
                foreach (var thumb in thumbs)
                {
                    var src = thumb.GetAttributeValue("src", "");
                    if (src != "")
                    {
                        if (keyword != "")
                        {
                            if (src.ToLower().Contains(keyword.ToLower()))
                            {
                                this.results.Add(src.Replace("_320px", ""));
                            }
                        }
                        else 
                        {
                            this.results.Add(src.Replace("_320px", ""));
                        }
                    }
                }
            }
        }

        private void SendLogMessage(string message, LogSeverity severity, ref IRabbitMqService mqClient)
        {
            LogDto log = new LogDto
            {
                Message = message,
                Severity = severity
            };
            mqClient.SendMessage(JsonConvert.SerializeObject(log), "LogMessageQueue");
        }
    }
}
