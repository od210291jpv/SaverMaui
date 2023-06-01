namespace SaverMaui.Services.Contracts.Video
{
    public class AddVideoRequestDto
    {
        public string Title { get; set; } = string.Empty;

        public string ImageUri { get; set; } = string.Empty;

        public Guid? CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public Guid? PublisherProfileId { get; set; }

    }
}
