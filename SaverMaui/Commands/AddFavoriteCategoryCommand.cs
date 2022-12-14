using Realms;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class AddFavoriteCategoryCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly CategoriesViewModel vm;

        public AddFavoriteCategoryCommand(CategoriesViewModel viewModel)
        {
            this.vm = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (vm != null)
            {
                return true;
            }

            return false;
        }

        public async void Execute(object parameter)
        {
            Realm _realm = Realm.GetInstance();
            var reqCatId = Environment.SahredData.currentCategory.CategoryId;

            var requiredCategory = _realm.All<Category>().Single(ct => ct.CategoryId == Environment.SahredData.currentCategory.CategoryId);

            if (requiredCategory.IsFavorite == false) 
            {
                _realm.Write(() => requiredCategory.AmountOfFavorites += 1);
            }

            _realm.Write(() => requiredCategory.IsFavorite = !requiredCategory.IsFavorite);

            try
            {
                var rCategory = _realm.All<Category>().Single(ct => ct.CategoryId == Environment.SahredData.currentCategory.CategoryId);

                await BackendServiceClient
                    .GetInstance()
                    .UpdateCategoryStatisticsAsync(
                    requiredCategory.CategoryId,
                    requiredCategory.AmountOfOpenings,
                    requiredCategory.AmountOfFavorites);
            }
            catch { }

            await Application.Current.MainPage.DisplayAlert("Done", $"Category Added/Removed as favorite: {requiredCategory.Name}", "Ok");
        }
    }
}
