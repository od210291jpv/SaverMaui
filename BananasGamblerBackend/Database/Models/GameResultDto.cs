namespace BananasGamblerBackend.Database.Models
{
    public class GameResultDto
    {
        public string Result { get; set; } = string.Empty;

        public RewardsDto? Rewards { get; set; }
    }
}
