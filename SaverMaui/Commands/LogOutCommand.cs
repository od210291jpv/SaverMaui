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
            return IsOnlineHelper.IsOnline && Environment.Login != null;
        }

        public async void Execute(object parameter)
        {
            await BackendServiceClient.GetInstance().LogoutUserAsync();

            if (await BackendServiceClient.GetInstance().IsUserLoggedInAsync(Environment.Login) == false)
            {
                Environment.Login = null;
                Environment.Password = null;

                await Application.Current.MainPage.DisplayAlert("Done", $"You logged out!!", "Ok");
            }
            else 
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Something went wrong!", "Ok");
            }
        }
    }
}
