using System.ComponentModel.DataAnnotations.Schema;

namespace SaverBackend.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
        
        public int ProfileId { get; set; }

        public Profile? Profile { get; set; }

        public int? AmountOfOpenings { get; set; }

        public int? AmountOfFavorites { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
