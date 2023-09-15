namespace BananasGambler.DTO
{
    internal class LoginResponseDto
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public decimal Credits { get; set; }
    }
}
