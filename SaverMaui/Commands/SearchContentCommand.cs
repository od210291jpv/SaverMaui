using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SaverMaui.Services;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class SearchContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private SettingsViewModel vm;

        public SearchContentCommand(SettingsViewModel viewModel)
        {
            this.vm = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (Environment.Login != null && Environment.Login != "") 
            {
                return true;
            }
            return false;
        }

        public async void Execute(object parameter)
        {
            var result = await BackendServiceClient.GetInstance().ContentActions.SearchContent(this.vm.SearchRequest);
            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make($"Search requested", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
            }
            else 
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make($"Someting went wrong! Status code {result.StatusCode}", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
            }

            this.vm.SearchRequest = "";
        }
    }
}
