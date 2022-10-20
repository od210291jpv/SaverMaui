using SaverMaui.Commands;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    internal class LoginPageViewModel : BaseViewModel
    {
        public ICommand LoginCommand { get; }

        public ICommand RegisterCommand { get; }

        public bool IsUserLoggedIn { get; set; }

        public LoginPageViewModel()
        {
            this.LoginCommand = new LoginCommand(this);
            this.RegisterCommand = new RegisterUserCommand(this);
        }

        public string Login { get; internal set; }
        public string Password { get; internal set; }
    }
}
