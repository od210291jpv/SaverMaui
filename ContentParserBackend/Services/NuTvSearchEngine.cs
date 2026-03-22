
using ContentParserBackend.Dto;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace ContentParserBackend.Services
{
    public class NuTvSearchEngine
    {
        private string url = string.Empty;
        private string startLetter;
        private RestClient restClient;

        public NuTvSearchEngine(string url, string startLetter)
        {
            this.url = url;
            this.restClient = new RestClient(this.url);
            this.startLetter = startLetter;
        }

        public async Task ParseAsync(string keyword, IRabbitMqService mqClient, IDatabase? redis)
        {
            SendLogMessageAsync("Starting nutv engine thread..", LogSeverity.Verbose, mqClient);

            var intermediateLinks = new ConcurrentBag<string>();
            var semaphore = new SemaphoreSlim(4); 
            var tasks = new List<Task>();

            // open the start letter page
            RestRequest request = new RestRequest($"models/{this.startLetter}/1/", Method.Get);

            SendLogMessageAsync($"Parsing content for {startLetter} start letter", LogSeverity.Verbose, mqClient);
            var response = await this.restClient.ExecuteGetAsync<string>(request);
            var responseContent = response.Content;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseContent);
            // get the pages amount

            int paginationHlderRetryAmount = 10;
            HtmlNode? paginationHolder = default;

            try
            {
                paginationHolder = document.DocumentNode.SelectNodes("//div[@class = 'pagination-holder']").FirstOrDefault();
                if (paginationHolder is null) 
                {
                    throw new Exception("Pagination holder not found on the page");
                }
            }
            catch(Exception e)
            {
                SendLogMessageAsync($"Get pahination holder for letter {startLetter}: failed, Error {e.Message}, wiil attempt to retry", LogSeverity.Error, mqClient);
                while (paginationHlderRetryAmount > 0)
                {
                    try
                    {
                        paginationHolder = document.DocumentNode.SelectNodes("//div[@class = 'pagination-holder']").FirstOrDefault();
                        break;
                    }
                    catch (Exception ex)
                    {
                        SendLogMessageAsync($"Get pahination holder for letter {startLetter}: failed, Error {ex.Message}, retry attempts left: {paginationHlderRetryAmount}", LogSeverity.Error, mqClient);
                        paginationHlderRetryAmount--;
                        await Task.Delay(5000);
                    }
                }
            }

            if (paginationHolder is null) 
            {
                SendLogMessageAsync($"Get pahination holder for letter {startLetter}: failed after retries, stopping parsing for the letter", LogSeverity.Error, mqClient);
                return;
            }

            var amountOfPages = paginationHolder.SelectNodes("//ul/li").ToArray().Length;
            SendLogMessageAsync($"Amount of pages for the letter {startLetter}: {amountOfPages}", LogSeverity.Verbose, mqClient);

            // parse preview links on each page and put in links collection
            SendLogMessageAsync($"Parsing preview linkns for {startLetter} keyword {keyword}", LogSeverity.Verbose, mqClient);

            for (int i = 1; i <= amountOfPages; i++) 
            {
                int pageNumber = i;

                tasks.Add(Task.Run(async () => 
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        RestRequest pageRequest = new RestRequest($"models/{this.startLetter}/{i}/", Method.Get);
                        var pageResponse = await this.restClient.ExecuteGetAsync<string>(pageRequest);

                        HtmlDocument pageDocument = new HtmlDocument();
                        pageDocument.LoadHtml(pageResponse.Content);

                        var imagesGrid = pageDocument.GetElementbyId("list_videos_related_videos_items");
                        var previewNodes = imagesGrid?.SelectNodes("//div[@class = 'item']/a").ToArray();
                        if (previewNodes != null)
                        {
                            foreach (var node in previewNodes)
                            {
                                var link = node?.GetAttributeValue("href", "");
                                SendLogMessageAsync($"Found preview link {link} checking against {keyword} with start letter {startLetter}", LogSeverity.Verbose, mqClient);
                                if (!string.IsNullOrEmpty(link) && link.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                {
                                    SendLogMessageAsync($"Found mathing link {link} with keyword {keyword}. Adding intermediate link", LogSeverity.Warn, mqClient);
                                    intermediateLinks.Add(link);
                                }
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            SendLogMessageAsync($"Starting parsing {intermediateLinks.Count()} intermediate links", LogSeverity.Verbose, mqClient);
            foreach (var link in intermediateLinks)
            {
                //SendLogMessage($"Parsing intermediate link: {link}", LogSeverity.Verbose, ref mqClient);
                RestRequest contentRequest = new RestRequest(link, Method.Get);
                var contentResponse = await this.restClient.ExecuteGetAsync<string>(contentRequest);
                var contentPage = contentResponse.Content;
                HtmlDocument contentDocument = new HtmlDocument();
                contentDocument.LoadHtml(contentPage);

                SendLogMessageAsync("Parsing thumbs", LogSeverity.Warn, mqClient);
                await this.ParseThumbs(contentDocument, keyword, redis, mqClient);

                var hasNext = contentDocument.DocumentNode.SelectSingleNode("//li[@class = 'next']") != null;

                while (hasNext)
                {
                    SendLogMessageAsync($"Intermediate link has next page, parsing.. has next: {hasNext}", LogSeverity.Verbose, mqClient);
                    var nextLink = contentDocument.DocumentNode.SelectSingleNode("//li[@class = 'next']/a")?.GetAttributeValue("href", "");
                    RestRequest nextContentRequest = new RestRequest(nextLink, Method.Get);
                    var nextContentResponse = await this.restClient.ExecuteGetAsync<string>(nextContentRequest);
                    var nextContentPage = nextContentResponse.Content;
                    contentDocument.LoadHtml(nextContentPage);

                    await this.ParseThumbs(contentDocument, keyword, redis, mqClient);

                    hasNext = contentDocument.DocumentNode.SelectSingleNode("//li[@class = 'next']") != null;
                }
            }
            SendLogMessageAsync($"Done parsing! ", LogSeverity.Warn, mqClient);
        }

        private async Task ParseThumbs(HtmlDocument html, string keyword, IDatabase redis, IRabbitMqService mqClient)
        {
            var thumbs = html.DocumentNode.SelectNodes("//img[@class = 'thumb']");
            SendLogMessageAsync($"Found {thumbs.Count()} to parse", LogSeverity.Warn, mqClient);

            if (thumbs != null)
            {
                foreach (var thumb in thumbs)
                {
                    var src = thumb.GetAttributeValue("src", "");
                    if (src != "")
                    {
                        SendLogMessageAsync($"Parsing thumb: {src}", LogSeverity.Warn, mqClient);

                        if (keyword != "")
                        {
                            if (src.ToLower().Contains(keyword.ToLower()))
                            {
                                SendLogMessageAsync($"Found match {src} to the keyword {keyword}, sending to redis..", LogSeverity.Warn, mqClient);
                                await redis!.StringSetAsync($"{Guid.NewGuid().ToString()}:{keyword}", src.Replace("_320px", ""));
                                await redis.ListLeftPushAsync("logstash-parsing-logs", JsonConvert.SerializeObject(new SarchLogDto 
                                {
                                    Keyword = keyword,
                                    Url = src.Replace("_320px", ""),
                                    Timestamp = DateTime.UtcNow
                                }));
                            }
                        }
                        else 
                        {
                            SendLogMessageAsync($"Found {src}, sending to redis..", LogSeverity.Warn, mqClient);
                            await redis!.StringSetAsync($"{Guid.NewGuid().ToString()}:{keyword}", src.Replace("_320px", ""));
                            await redis.ListLeftPushAsync("logstash-parsing-logs", JsonConvert.SerializeObject(new SarchLogDto 
                            {
                                Keyword = keyword,
                                Url = src.Replace("_320px", ""),
                                Timestamp = DateTime.UtcNow
                            }));
                        }
                    }
                }
                SendLogMessageAsync($"Done thumbs parsing!", LogSeverity.Warn, mqClient);
            }
            else
            {
                SendLogMessageAsync($"No thumbs found on the page", LogSeverity.Warn, mqClient);
            }
        }

        private void SendLogMessageAsync(string message, LogSeverity severity, IRabbitMqService mqClient)
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
