using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;

using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class AddContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> execute;
        private SettingsViewModel viewModel;

        public AddContentCommand(SettingsViewModel vm, Action<object> _execute)
        {
            this.execute = _execute;
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            Realm _realm = Realm.GetInstance();

            if (_realm != null)
            {
                return true;
            }
            return false;
        }

        public async void Execute(object parameter)
        {
            this.execute(parameter);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"New content added!", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
            this.viewModel.ContentUri = "";

            SettingsViewModel.GetInstance().ContentAmount += 1;
        }
    }

}
