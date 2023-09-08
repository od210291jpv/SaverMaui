using System.ComponentModel.DataAnnotations.Schema;

namespace BananasGamblerBackend.Database.SaverBackendModels
{
    public class Content
    {
        [ForeignKey(nameof(Id))]
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImageUri { get; set; }

        public Guid? CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int? ProfileId { get; set; }

        public Profile? Profile { get; set; }
    }
}
