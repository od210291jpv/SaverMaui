using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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

            var requiredCategory = _realm.All<Category>().Single(ct => ct.CategoryId == this.vm.SelectedCategory.CategoryId);

            if (requiredCategory.IsFavorite == false) 
            {
                _realm.Write(() => requiredCategory.AmountOfFavorites += 1);
            }

            _realm.Write(() => requiredCategory.IsFavorite = !requiredCategory.IsFavorite);

            try
            {
                var rCategory = _realm.All<Category>().Single(ct => ct.CategoryId == Environment.SahredData.currentCategory.CategoryId);

                _ = await BackendServiceClient
                    .GetInstance()
                    .UpdateCategoryStatisticsAsync(
                    requiredCategory.CategoryId,
                    requiredCategory.AmountOfOpenings,
                    requiredCategory.AmountOfFavorites);
            }
            catch { }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"Category Added/Removed as favorite: {requiredCategory.Name}", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
