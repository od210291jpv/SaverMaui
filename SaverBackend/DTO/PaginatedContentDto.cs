using SaverBackend.Models;

namespace SaverBackend.DTO
{
    public class PaginatedContentDto
    {
        public List<Content> Items { get; set; } = new();

        public int TotalCount { get; set; } = 0;

        public bool HasNextPage => (Items.Count + (TotalCount - Items.Count)) > Items.Count;
            
        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }
    }
}
