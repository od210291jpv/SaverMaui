using Microsoft.EntityFrameworkCore;

namespace BananasGamblerBackend.Database.Models
{
    [PrimaryKey(nameof(Id))]
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
