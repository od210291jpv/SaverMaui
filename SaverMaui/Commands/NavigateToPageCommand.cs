using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
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
            Environment.SahredData.currentCategory = this._vm.SelectedCategory;

            var category = Realm.GetInstance().All<Category>().Single(c => c.CategoryId == this._vm.SelectedCategory.CategoryId);

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
