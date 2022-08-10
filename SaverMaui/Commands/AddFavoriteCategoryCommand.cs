using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            _realm.Write(() => requiredCategory.IsFavorite = !requiredCategory.IsFavorite);

            await Application.Current.MainPage.DisplayAlert("Done", $"Category Added/Removed as favorite: {requiredCategory.Name}", "Ok");
        }
    }
}
