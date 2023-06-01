using System.ComponentModel.DataAnnotations.Schema;

namespace SaverBackend.Models
{
    public class VideoContent
    {
        [ForeignKey(nameof(Id))]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string VideoUri { get; set; } = string.Empty;

        public Guid? CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int? ProfileId { get; set; }

        public Profile? Profile { get; set; }
    }
}
