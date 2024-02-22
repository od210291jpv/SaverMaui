using BananasGambler.DTO;
using BananasGambler.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananasGambler.ViewModels
{
    internal class CategoryFeedViewModel : BaseViewModel
    {
        private static CategoryFeedViewModel instance;

        public static CategoryFeedViewModel GetInstance()
        {
            if (instance is null)
            {
                instance = new CategoryFeedViewModel();
            }

            return instance;
        }

        private ObservableCollection<GameCardDto> cards { get; set; }
        
        public ObservableCollection<GameCardDto> Cards { get => this.cards; set => this.cards = value; }

        private GameCardDto currentCard { get; set; }

        public GameCardDto CurrentCard { get => this.currentCard; set => this.currentCard = value; }

        public CategoryFeedViewModel()
        {
            instance = this;
            this.cards = new ObservableCollection<GameCardDto>();
            this.Cards = new ObservableCollection<GameCardDto>();
            currentCard = new GameCardDto();
            CurrentCard = new GameCardDto();


            if (GlobalData.UserData.Login != "" && GlobalData.UserData.Password != "")
            {
                var httpClient = HttpClientService.GetInstance();
                var selectedPack = PacksPageViewModel.Instance.SelectedPack;

                var cards = httpClient.GetUserCards(new LoginRequestDto()
                {
                    Login = GlobalData.UserData.Login,
                    Password = GlobalData.UserData.Password
                }).Where(c => c.CategoryId == selectedPack.CategoryId);

                foreach (var crd in cards)
                {
                    this.Cards.Add(crd);
                }

                this.CurrentCard = this.Cards.FirstOrDefault();
            }
        }
    }
}
