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
    internal class PacksPageViewModel : BaseViewModel
    {
        internal static PacksPageViewModel Instance { get; private set; }

        public ObservableCollection<CategoryDto> Packs = new ObservableCollection<CategoryDto>();

        private CategoryDto selectedPack;

        public CategoryDto SelectedPack
        {
            get { return selectedPack; }
            set
            {
                selectedPack = value;
                OnPropertyChanged("SelectedPack");
            }
        }


        public PacksPageViewModel()
        {
            Instance = this;

            var userPacksIds = HttpClientService.GetInstance()
                .GetUserCards(
                new LoginRequestDto() 
                { 
                    Login = GlobalData.UserData.Login,
                    Password = GlobalData.UserData.Password
                })
                .Select(x => x.CategoryId)
                .ToArray();

            var allCategories = HttpClientService.GetInstance().GetCardsCategories();
            var userPacksCategories = allCategories.Where(c => userPacksIds.Contains(c.CategoryId) == true).ToArray();

            foreach (var c in userPacksCategories) 
            {
                this.Packs.Add(c);
            }
        }
    }
}
