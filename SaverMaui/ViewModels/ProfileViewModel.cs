using SaverMaui.Commands;
using SaverMaui.Services.Helpers;

using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    internal class ProfileViewModel : BaseViewModel
    {

        public string Login { get; set; }

        public string Password { get; set; }

        public bool WeAreOnline { get; set; }

        public string UserName { get; set; }

        public Color OnlineButtonBackgroundColor { get; set; } = new Color(255, 160, 122);

        private string resultsAmnt;

        public string ResultsAmnt 
        { 
            get => resultsAmnt;
            set 
            { 
                resultsAmnt = value; 
                OnPropertyChanged("ResultsAmnt"); 
            } 
        }

        public ProfileViewModel()
        {
            this.WeAreOnline = IsOnlineHelper.IsOnline;

            if (this.WeAreOnline) 
            {
                this.OnlineButtonBackgroundColor = new Color(124, 252, 0);
                this.UserName = Environment.Login;
            }

            this.ResultsAmnt = "0";
        }
    }
}
