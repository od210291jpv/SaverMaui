using Realms;
using SaverMaui.ViewModels;
using SaverMaui.Views;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class NavigateToPersonalFeedCommand : ICommand
    {
        private Realm realmInstance;
        private FavoritesViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public NavigateToPersonalFeedCommand(FavoritesViewModel vm)
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
            await Application.Current.MainPage.Navigation.PushAsync(new PersonalFeedPage());
        }
    }
}
