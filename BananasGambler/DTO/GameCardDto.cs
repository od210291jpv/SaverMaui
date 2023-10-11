namespace BananasGambler.DTO
{
    public class GameCardDto : BaseNotifyingDto
    {
        private bool isNewItem { get; set; }

        public bool IsNewItem 
        {
            get => this.isNewItem;
            set 
            {
                this.isNewItem = value;
                OnPropertyChanged(nameof(this.isNewItem));
            }
        }

        public int Id { get; set; }

        public string CardTitle { get; set; } = string.Empty;

        public string ImageUri { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int? userId { get; set; }

        public object users { get; set; }

        public decimal CostInCredits { get; set; }

        public decimal Rarity { get; set; } = decimal.Zero;
    }
}
