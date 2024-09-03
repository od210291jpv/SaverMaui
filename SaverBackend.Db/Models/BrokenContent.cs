namespace SaverBackend.Models
{
    public class BrokenContent
    {
        public Guid? CategoryId { get; set; }

        public int Id { get; set; }

        public string ImageUri { get; set; } 

        public string Title { get; set; }
    }
}