namespace LiveHost.DataBase.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public decimal Credits { get; set; }

        public List<GameCard> Cards { get; set; } = new List<GameCard>();
    }
}
