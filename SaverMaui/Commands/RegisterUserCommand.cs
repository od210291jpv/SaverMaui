using SaverMaui.Services;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.ServiceExtensions;

using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class RegisterUserCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        
        private LoginPageViewModel viewModel;

        public RegisterUserCommand(LoginPageViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return IsOnlineHelper.IsOnline;
        }

        public async void Execute(object parameter)
        {
            var status = await BackendServiceClient
                .GetInstance()
                .RegisterUser(viewModel.Login, viewModel.Password);

            if (status == System.Net.HttpStatusCode.OK)
            {
                await Application.Current.MainPage.DisplayAlert("Done", $"You registered!!", "Ok");
            }
            else 
            {
                await Application.Current.MainPage.DisplayAlert("Error!", $"Something went wrong!", "Ok");
            }
        }
    }
}
