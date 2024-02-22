using BananasGambler.Commands;
using BananasGambler.DTO;
using BananasGambler.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BananasGambler.ViewModels
{
    internal class PlayGameViewModel : BaseViewModel
    {
        internal static PlayGameViewModel Instance;

        public ObservableCollection<GameCardDto> Cards { get; set; }

        private GameCardDto bidCard { get; set; }

        public GameCardDto BidCard 
        {
            get => bidCard;
            set 
            {
                bidCard = value;
                OnPropertyChanged(nameof(BidCard));
            }
        }

        private string bidCardName { get; set; }

        public string BidCardName 
        { 
            get => this.bidCardName;
            set 
            {
                this.bidCardName = value;
                OnPropertyChanged(nameof(BidCardName));
            }
        }

        private string bidCardRarity { get; set; }

        public string BidCardRarity 
        {
            get => this.bidCardRarity;
            set 
            {
                this.bidCardRarity = value;
                OnPropertyChanged(nameof(BidCardRarity));
            }
        }

        private string bidCardPrice { get; set; }

        public string BidCardPrice 
        {
            get => this.bidCardPrice;
            set 
            {
                this.bidCardPrice = value;
                OnPropertyChanged(nameof(BidCardPrice));
            }
        }

        private string currentBalance { get; set; }

        public string CurrentBalance 
        {
            get => this.currentBalance;
            set 
            {
                this.currentBalance = value;
                OnPropertyChanged(nameof(CurrentBalance));
            }
        }

        public int ValueToPlay { get; set; }

        private string valueOne { get; set; }
        private string valueTwo { get; set; }
        private string valueThree { get; set; }

        public string ValueOne 
        {
            set 
            {
                this.valueOne = value;
                this.OnPropertyChanged(nameof(this.ValueOne));
            }
            get => this.valueOne;
        }

        public string ValueTwo
        {
            set
            {
                this.valueTwo = value;
                this.OnPropertyChanged(nameof(this.ValueTwo));
            }
            get => this.valueTwo;
        }

        public string ValueThree
        {
            set
            {
                this.valueThree = value;
                this.OnPropertyChanged(nameof(this.ValueThree));
            }
            get => this.valueThree;
        }

        private bool btnOneEnabled { get; set; }

        private bool btnTwoEnabled { get; set; }

        private bool btnThreeEnabled { get; set; }

        public bool BtnOneEnabled 
        {
            get => this.btnOneEnabled;
            set 
            {
                this.btnOneEnabled = value;
                this.OnPropertyChanged(nameof(this.BtnOneEnabled));
            }
        }
        public bool BtnTwoEnabled 
        {
            get => this.btnTwoEnabled;
            set 
            {
                this.btnTwoEnabled = value;
                this.OnPropertyChanged(nameof(this.BtnTwoEnabled));
            }
        }

        public bool BtnThreeEnabled 
        {
            get => this.btnThreeEnabled;
            set 
            {
                this.btnThreeEnabled = value;
                this.OnPropertyChanged(nameof(this.BtnThreeEnabled));
            }
        }

        private string status { get; set; }

        public string Status 
        {
            get => this.status;
            set 
            {
                this.status = value;
                OnPropertyChanged(nameof(this.Status));
            }
        }

        public ICommand PlayCommand { get; set; }

        public ICommand PlayValueOneCommand { get; set; }

        public ICommand PlayValueTwoCommand { get; set; }

        public ICommand PlayValueThreeCommand { get; set; }

        public ICommand BuyOneCommand { get; set; }

        public ICommand BuyThreeCommand { get; set; }

        public ICommand BuyFiveCommand { get; set; }

        public ICommand BuySevenCommand { get; set; }

        public ICommand IsPassCommand { get; set; }

        public PlayGameViewModel()
        {
            Instance = this;

            this.PlayCommand = new PlayGameCommand(this);
            this.PlayValueOneCommand = new PlayValueOneCommand(this);
            this.PlayValueTwoCommand = new PlayValueTwoCommand(this);
            this.PlayValueThreeCommand = new PlayValueThreeCommand(this);
            this.BuyOneCommand = new ByOneCommand(this);
            this.BuyThreeCommand = new BuyThreeCommand(this);
            this.BuyFiveCommand = new BuyFiveCommand(this);
            this.BuySevenCommand = new BuySevenCommand(this);
            this.IsPassCommand = new PassCommand(this);

            this.InitGameValues();

            this.Cards = new ObservableCollection<GameCardDto>();

            if (GlobalData.UserData.Login != "" && GlobalData.UserData.Password != "") 
            {
                var httpClient = HttpClientService.GetInstance();
                var cards = httpClient.GetUserCards(new LoginRequestDto() 
                {
                    Login = GlobalData.UserData.Login,
                    Password = GlobalData.UserData.Password
                }).OrderBy(c => c.CardTitle);

                foreach (var card in cards) 
                {
                    this.Cards.Add(card);
                }

            }

            this.Status = "Game Not Started";

            this.BtnThreeEnabled = true;
            this.btnTwoEnabled = true;
            this.btnThreeEnabled = true;
        }

        internal void InitGameValues() 
        {
            this.ValueOne = "1";
            this.ValueTwo = new Random().Next(2, 5).ToString();
            this.ValueThree = new Random().Next(1, 7).ToString();
        }

        public void LockGameButtons(bool unLock) 
        {
            this.btnOneEnabled = unLock;
            this.btnTwoEnabled = unLock;
            this.btnThreeEnabled = unLock;
        }

        public async Task RefreshCardsAsync() 
        {
            this.Cards.Clear();

            if (GlobalData.UserData.Login != "" && GlobalData.UserData.Password != "")
            {
                var httpClient = HttpClientService.GetInstance();
                var cards = await httpClient.GetUserCardsAsync(new LoginRequestDto()
                {
                    Login = GlobalData.UserData.Login,
                    Password = GlobalData.UserData.Password
                });

                foreach (var card in cards.OrderBy(c => c.CardTitle))
                {
                    this.Cards.Add(card);
                }

                if (cards.Length > 0) 
                {
                    this.bidCard = cards.First();
                }
            }
        }
    }
}
