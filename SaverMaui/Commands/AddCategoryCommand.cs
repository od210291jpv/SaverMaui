using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
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

            Realm _realm = Realm.GetInstance();
            await _realm.WriteAsync(() => _realm.Add(category));

            this.viewModel.Categories = _realm.All<Category>().OrderBy(c => c.Name).ToObservableCollection();

            viewModel.NewCategoryName = "";

            SettingsViewModel.GetInstance().CategoriesAmount += 1;

            if (CategoriesViewModel.Instance != null) 
            {
                CategoriesViewModel.Instance.Categories = this.viewModel.Categories;
            }

            if (DeviceInfo.Current.Platform.ToString().ToLower() == "android") 
            {
                HapticFeedback.Perform(HapticFeedbackType.Click);
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"Category added!", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
