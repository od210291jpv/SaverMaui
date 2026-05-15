namespace SaverBackend.DTO
{
    public class KeywordResult
    {
        public string Key { get; set; } = string.Empty;

        public List<string> Urls { get; set; } = new();
    }
}
