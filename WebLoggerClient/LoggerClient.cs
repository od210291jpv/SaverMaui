namespace WebLoggerClient
{
    public class LoggerClient
    {
        private string url;

        public LoggerClient(string url)
        {
            this.url = url;
        }

        public async Task LogAsync(string message, LogSeverity severity)
        {
            var logDto = new DTO.LogDto
            {
                Message = message,
                Severity = severity
            };
            using (var httpClient = new HttpClient())
            {
                var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(logDto), System.Text.Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"{url}/api/logs", jsonContent);
            }
        }
    }
}
