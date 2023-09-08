using Microsoft.EntityFrameworkCore;

namespace BananasGamblerBackend.Database.Models
{
    [PrimaryKey(nameof(CardsId))]
    public class GameCardUser
    {
        public int CardsId { get; set; }

        public int UsersId { get; set; }
    }
}
