namespace SaverBackend.DTO
{
    public class KeywordResult
    {
        public string Key { get; set; } = string.Empty;

        public List<KeyValuePair<int, string>> Urls { get; set; } = new();
    }
}
