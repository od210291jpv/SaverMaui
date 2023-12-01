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

        public ObservableCollection<CategoryDto> Packs { get; set; }

        private CategoryDto selectedPack { get; set; }

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
            this.Packs = new ObservableCollection<CategoryDto>();

            Guid[] userPacksIds = HttpClientService.GetInstance()
                .GetUserCards(
                new LoginRequestDto() 
                { 
                    Login = GlobalData.UserData.Login,
                    Password = GlobalData.UserData.Password
                })
                .Select(x => x.CategoryId)
                .Distinct()
                .ToArray();

            CategoryDto[] allCategories = HttpClientService.GetInstance().GetCardsCategories();
            List<CategoryDto> userPacksCategories = new List<CategoryDto>();

            foreach (var i in userPacksIds) 
            {
                var catToAdd = allCategories.FirstOrDefault(c => c.CategoryId == i);
                if (catToAdd != null) 
                {
                    userPacksCategories.Add(catToAdd);
                }
            }

            foreach (var c in userPacksCategories) 
            {
                this.Packs.Add(c);
            }

        }

        public async void OnCategoryOpen(object sender, EventArgs e)
        {
            //if (Environment.Login != null)
            //{
            //    this.NavigateToFeedItemCommand.Execute(CategoriesViewModel.Instance);
            //}
            //else
            //{
            //    await Application.Current.MainPage.DisplayAlert("Unauthorized", $"Please login or create account to use categories functionality!", "Ok");
            //}
        }
    }
}
