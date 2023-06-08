using Realms;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class WipeAllDataCommand : ICommand
    {
        private Realm realmInstance;

        public event EventHandler CanExecuteChanged;

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
            bool answer = await Application.Current.MainPage.DisplayAlert("Attention", "Would you like to delete all content", "Yes", "No");

            if (answer == true) 
            {
                this.realmInstance.Write(() => this.realmInstance.RemoveAll());
                await Application.Current.MainPage.DisplayAlert("Done", $"All data was deleted", "Ok");
            }
        }
    }
}
