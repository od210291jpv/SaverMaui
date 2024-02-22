using BananasGambler.Commands;
using BananasGambler.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BananasGambler.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        public ICommand LoginCommand { get; }

        public string Login { get; set; }
        public string Password { get; set; }

        private string status;

        public string Status 
        { 
            get => this.status;
            set 
            {
                this.status = value;
                this.OnPropertyChanged(nameof(Status));
            }
        }

        public LoginViewModel()
        {
            this.LoginCommand = new LoginCommand(this);
        }
    }
}
