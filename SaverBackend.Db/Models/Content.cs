using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaverBackend.Models
{
    public class Content
    {
        [ForeignKey(nameof(Id))]
        public int Id { get; set; }

        public int CmsExternalId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ImageUri { get; set; } = string.Empty;

        public Guid? CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int? ProfileId { get; set; }

        public List<Profile> Profile { get; set; } = new List<Profile>();

        public short Rating { get; set; } = 0;

        public decimal Cost { get; set; } = 0m;

        public bool IsPublic { get; set; } = true;

        public bool IsEnabled { get; set; } = true;

        public bool IsDeleted { get; set; } = true;

        public bool IsAiGenerated { get; set; } = true;
    }
}
