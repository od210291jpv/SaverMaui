using System;
using System.Collections.Generic;
using System.Text;

namespace SaverMaui.Services.Contracts.Search
{
    public class KeywordResult
    {
        public string Key { get; set; } = string.Empty;

        public List<KeyValuePair<int, string>> Urls { get; set; } = new();
    }
}
