using System.ComponentModel.DataAnnotations.Schema;

namespace BananasGamblerBackend.Database.Models
{
    public class GameCard
    {
        [ForeignKey(nameof(Id))]
        public int Id { get; set; }

        public string CardTitle { get; set; } = string.Empty;

        public string ImageUri { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; }

        public User? User { get; set; }
    }
}
