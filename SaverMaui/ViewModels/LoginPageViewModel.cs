using SaverMaui.Commands;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        public ICommand LoginCommand { get; }

        public ICommand RegisterCommand { get; }

        public bool IsUserLoggedIn { get; set; }

        public LoginPageViewModel()
        {
            this.LoginCommand = new LoginCommand(this);
            this.RegisterCommand = new RegisterUserCommand(this);
        }

        public string Login { get; set; }
        public string Password { get; set; }
    }
}
