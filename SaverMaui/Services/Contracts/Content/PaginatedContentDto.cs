using System;
using System.Collections.Generic;
using System.Text;

namespace SaverMaui.Services.Contracts.Content
{
    public class PaginatedContentDto
    {
        public List<ContentDto> Items { get; set; } = new();

        public int TotalCount { get; set; } = 0;

        public bool HasNextPage => (Items.Count + (TotalCount - Items.Count)) > Items.Count;

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }
    }
}
