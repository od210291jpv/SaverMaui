namespace BananasGamblerBackend.Dto
{
    public class UserDto
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public decimal Credits { get; set; }

        public List<GameCardDto> Cards { get; set; } = new List<GameCardDto>();
    }
}
