using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SaverMaui.Services;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class CleanSearchResultsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

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
            var result = await BackendServiceClient.GetInstance().ContentActions.DeleteSearchResults();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make($"Dome, results cleaned", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
            }
            else 
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make($"Someting went wrong! Status code {result.StatusCode}", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
            }
        }
    }
}
