using SaverMaui.Services;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class LogOutCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private ProfileViewModel viewModel;

        public LogOutCommand(ProfileViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            bool isLoggedIn = BackendServiceClient.GetInstance().IsUserLoggedInAsync(viewModel.Login).Result;
            return IsOnlineHelper.IsOnline && isLoggedIn;
        }

        public async void Execute(object parameter)
        {
            await BackendServiceClient.GetInstance().LoginUserAsync(this.viewModel.Login, this.viewModel.Password);
        }
    }
}
