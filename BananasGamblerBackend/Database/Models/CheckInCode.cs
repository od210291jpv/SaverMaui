namespace BananasGamblerBackend.Database.Models
{
    public class CheckInCode
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public bool IsActive { get; set; }
    }
}
