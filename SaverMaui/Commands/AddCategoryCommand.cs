using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class AddCategoryCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Realm realmInstance;

        private SettingsViewModel viewModel;

        public AddCategoryCommand(SettingsViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            Realm _realm = Realm.GetInstance();

            if (_realm != null)
            {
                this.realmInstance = _realm;
                return true;
            }
            return false;
        }

        public async void Execute(object parameter)
        {
            Category category = new Category()
            {
                CategoryId = Guid.NewGuid(),
                Name = (string)parameter,
                IsFavorite = false,
            };

            realmInstance.Write(() => realmInstance.Add(category));

            viewModel.NewCategoryName = "";

            CategoriesViewModel.Instance?.UpdateAllCategories();
            SettingsViewModel.GetInstance().UpdateAllCategories();

            SettingsViewModel.GetInstance().CategoriesAmount += 1;

            if (DeviceInfo.Current.Platform.ToString().ToLower() == "android") 
            {
                HapticFeedback.Perform(HapticFeedbackType.Click);
            }

            await Application.Current.MainPage.DisplayAlert("Done", $"Category Added: {category.Name}", "Ok");
        }
    }
}
