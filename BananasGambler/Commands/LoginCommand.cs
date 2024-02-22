using BananasGambler.Services;
using BananasGambler.ViewModels;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class LoginCommand : ICommand
    {
        private LoginViewModel viewModel;

        public LoginCommand(LoginViewModel vm)
        {
            this.viewModel = vm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (viewModel.Login != "" && viewModel.Password != "") 
            {
                return true;
            }
            return false;
        }

        public async void Execute(object parameter)
        {
            var client = HttpClientService.GetInstance();
            DTO.LoginResponseDto result = await client.LoginAsync(new DTO.LoginRequestDto() 
            {
                Login = viewModel.Login,
                Password = viewModel.Password,
            });

            GlobalData.UserData = result;

            if (GlobalData.UserData.Login != null && GlobalData.UserData.Password != null) 
            {
                this.viewModel.Status = "You successfully logged in";
            }
            else 
            {
                this.viewModel.Status = "Something went wrong";
            }
        }
    }
}
