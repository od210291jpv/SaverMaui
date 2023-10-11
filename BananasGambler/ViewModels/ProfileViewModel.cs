using BananasGambler.Commands;
using BananasGambler.Services;
using System.Windows.Input;

namespace BananasGambler.ViewModels
{
    class ProfileViewModel : BaseViewModel
    {
        private decimal credits { get; set; }

        public decimal Credits 
        {
            get => this.credits;
            set
            {
                this.credits = value;
                OnPropertyChanged(nameof(this.Credits));
            }
        }

        public ICommand BuyCardCommand { get; set; }

        public ProfileViewModel()
        {
            this.BuyCardCommand = new BuyACardCommand(this);

            if (GlobalData.UserData.Login != "" && GlobalData.UserData.Password != "") 
            {
                var client = HttpClientService.GetInstance();
                DTO.LoginResponseDto clientData = client.Login(new DTO.LoginRequestDto() { Login = GlobalData.UserData.Login, Password = GlobalData.UserData.Password });
                this.Credits = decimal.Round(clientData.Credits, 2);
            }
        }
    }
}
