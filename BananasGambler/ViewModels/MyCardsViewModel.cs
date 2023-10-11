using BananasGambler.Commands;
using BananasGambler.DTO;
using BananasGambler.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BananasGambler.ViewModels
{
    internal class MyCardsViewModel : BaseViewModel
    {
        internal static MyCardsViewModel Instance;

        public ObservableCollection<GameCardDto> Cards { get; set; }

        public ObservableCollection<int> NewCards { get; set; }

        private GameCardDto currentCard { get; set; }

        public GameCardDto CurrentCard 
        { 
            get => this.currentCard;
            set 
            {
                this.currentCard = value;
                OnPropertyChanged(nameof(this.CurrentCard));
            }
        }

        private bool isNewItem { get; set; }

        public bool IsNewItem
        {
            get => this.isNewItem;
            set
            {
                this.isNewItem = value;
                OnPropertyChanged(nameof(IsNewItem));
            }
        }

        public ICommand OnItemChangedCommand { get; set; }

        public MyCardsViewModel()
        {
            Instance = this;

            this.OnItemChangedCommand = new ItemChangedCommand(this);
            this.NewCards = new ObservableCollection<int>();

            this.Cards = new ObservableCollection<GameCardDto>();

            this.IsNewItem = false;

            if (GlobalData.UserData.Login != "" && GlobalData.UserData.Password != "")
            {
                var httpClient = HttpClientService.GetInstance();
                var cards = httpClient.GetUserCards(new LoginRequestDto()
                {
                    Login = GlobalData.UserData.Login,
                    Password = GlobalData.UserData.Password
                });

                foreach (var card in cards)
                {
                    card.CostInCredits = decimal.Round(card.CostInCredits, 2);
                    card.Rarity = decimal.Round(card.Rarity, 2);
                    this.Cards.Add(card);
                }
            }
        }

        public async Task RefreshCardsAsync() 
        {
            var oldCards = this.Cards.ToArray();

            this.Cards.Clear();
            var httpClient = HttpClientService.GetInstance();
            var cards = await httpClient.GetUserCardsAsync(new LoginRequestDto()
            {
                Login = GlobalData.UserData.Login,
                Password = GlobalData.UserData.Password
            });

            var newCards = cards.Where(c => oldCards.Select(f => f.Id).Contains(c.Id) == false).ToArray();

            foreach (var card in cards)
            {
                card.CostInCredits = decimal.Round(card.CostInCredits, 2);
                card.Rarity = decimal.Round(card.Rarity, 2);
                if (newCards.Contains(card) == true) 
                {
                    card.IsNewItem = true;
                }
                this.Cards.Add(card);
            }
        }
    }
}
