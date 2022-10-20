using Realms;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;
using SaverMaui.Views;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class NavigateToFavoriteCategoriesPageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private FavoritesViewModel _vm;

        public NavigateToFavoriteCategoriesPageCommand(FavoritesViewModel vm)
        {
            this._vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            Environment.SahredData.currentCategory = this._vm.SelectedFavoriteCategory;

            var category = Realm.GetInstance().All<Category>().Single(c => c.CategoryId == this._vm.SelectedFavoriteCategory.CategoryId);

            Realm.GetInstance().Write(() => category.AmountOfOpenings += 1);

            try
            {
                await BackendServiceClient.GetInstance()
                    .UpdateCategoryStatisticsAsync(category.CategoryId,
                    category.AmountOfOpenings,
                    category.AmountOfFavorites);
            }
            catch
            { }

            await Application.Current.MainPage.Navigation.PushAsync(new CategoryFeedPage());
        }
    }
}
