using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class AddVideoCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private VideoManagementViewModel viewModel;

        public AddVideoCommand(VideoManagementViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            Realm _realm = Realm.GetInstance();

            Video video = new Video()
            {
                CategoryId = Guid.NewGuid(),
                IsFavorite = false,
                Title = this.viewModel.AddVideoName,
                VideoUri = this.viewModel.AddVideoUrl
            };

            _realm.Write(() => _realm.Add(video));

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"New Video Added", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
            this.viewModel.AddVideoName = "";
            this.viewModel.AddVideoUrl = "";
        }
    }
}
