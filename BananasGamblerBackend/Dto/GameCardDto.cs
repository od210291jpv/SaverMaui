namespace BananasGamblerBackend.Dto
{
    public class GameCardDto
    {
        public int Id { get; set; }

        public string CardTitle { get; set; } = string.Empty;

        public string ImageUri { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public decimal CostInCredits { get; set; }
    }
}
