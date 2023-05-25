namespace SaverBackend.Models
{
    public class Content
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImageUri { get; set; }

        public Guid? CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
