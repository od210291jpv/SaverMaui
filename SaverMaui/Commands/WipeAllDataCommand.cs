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
            this.realmInstance.Write(() => this.realmInstance.RemoveAll());

            await Application.Current.MainPage.DisplayAlert("Done", $"All data was deleted", "Ok");
        }
    }
}
