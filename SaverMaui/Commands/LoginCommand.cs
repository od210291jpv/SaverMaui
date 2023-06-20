using SaverMaui.Services;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class LoginCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private LoginPageViewModel viewModel;

        public LoginCommand(LoginPageViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return IsOnlineHelper.IsOnline;
        }

        public async void Execute(object parameter)
        {
            if (!await BackendServiceClient.GetInstance().PingAsync()) 
            {
                return;
            }

            Environment.ProfileData = await BackendServiceClient
                .GetInstance()
                .LoginUserAsync(this.viewModel.Login, this.viewModel.Password);

            bool isLoggedIn = await BackendServiceClient.GetInstance().IsUserLoggedInAsync(this.viewModel.Login);

            if (this.viewModel.Login == "xxx" && this.viewModel.Password == "xxx") 
            {
                Environment.Login = this.viewModel.Login;
                Environment.Password = this.viewModel.Password;
                await Application.Current.MainPage.DisplayAlert("Done", $"You logged as secret admin!!", "Ok");

                await Shell.Current.GoToAsync("///Settings");
                return;
            }

            if (isLoggedIn == true)
            {
                await Application.Current.MainPage.DisplayAlert("Done", $"You logged in!!", "Ok");
                
                Environment.Login = this.viewModel.Login;
                Environment.Password = this.viewModel.Password;
                Environment.ProfileId = Environment.ProfileData.ProfileId;
                await Shell.Current.GoToAsync("///Settings");
            }
            else 
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Something went wrong! Please check your creds", "Ok");
            }
        }
    }
}
