using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;
using SaverMaui.Views;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class NavigateToPageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private CategoriesViewModel _vm;

        public NavigateToPageCommand(CategoriesViewModel vm)
        {
            this._vm = vm;
        }

        public bool CanExecute(object parameter)
        {            
            return true;
        }

        public async void Execute(object parameter)
        {


            if (IsOnlineHelper.IsOnline == true) 
            {
                try
                {
                    var r = Environment.SahredData.currentCategory;
                    await BackendServiceClient.GetInstance()
                        .UpdateCategoryStatisticsAsync(CategoriesViewModel.Instance.SelectedCategory.CategoryId,
                        this._vm.SelectedCategory.AmountOfOpenings ?? 0,
                        this._vm.SelectedCategory.AmountOfFavorites ?? 0);
                }
                catch
                { }
            }

            await Application.Current.MainPage.Navigation.PushAsync(new CategoryFeedPage());
        }
    }
}
