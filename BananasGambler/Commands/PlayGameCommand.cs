using BananasGambler.DTO;
using BananasGambler.Services;
using BananasGambler.ViewModels;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    class PlayGameCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private PlayGameViewModel viewModel;

        public PlayGameCommand(PlayGameViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (GlobalData.UserData.Login == "" | GlobalData.UserData.Password == "") 
            {
                this.viewModel.Status = "You are not authorized";
                return false;
            }
            if (this.viewModel == null) 
            {
                return false;
            }
            return true;
        }

        private void ResetGame() 
        {
            GlobalData.GameStarted = false;
            GlobalData.GameBid = 0;
            GlobalData.IsPass = false;
            this.viewModel.LockGameButtons(true);
            this.viewModel.BtnOneEnabled = true;
            this.viewModel.BidCardName = "";
            this.viewModel.BidCardPrice = "";
            this.viewModel.BidCardRarity = "";
        }

        public async void Execute(object parameter)
        {
            this.viewModel.InitGameValues();
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
            if (GlobalData.GameStarted == false) 
            {
                HttpClientService httpService = HttpClientService.GetInstance();
                var result = await httpService.StartGameAsync(
                    new LoginRequestDto() 
                    {
                        Login = GlobalData.UserData.Login,
                        Password = GlobalData.UserData.Password 
                    },
                    this.viewModel.BidCard.Id);

                if (result == System.Net.HttpStatusCode.OK) 
                {
                    GlobalData.GameStarted = true;
                    this.viewModel.Status = "Game session started.";

                    this.viewModel.BidCardName = this.viewModel.BidCard.CardTitle;
                    this.viewModel.BidCardPrice = decimal.Round(this.viewModel.BidCard.CostInCredits, 3).ToString();
                    this.viewModel.BidCardRarity = decimal.Round(this.viewModel.BidCard.Rarity, 3).ToString();
                    this.viewModel.CurrentBalance = decimal.Round(httpService.Login(new DTO.LoginRequestDto() { Login = GlobalData.UserData.Login, Password = GlobalData.UserData.Password }).Credits, 3).ToString();
                }

                GlobalData.IsPass = false;
                this.viewModel.LockGameButtons(true);
                return;
            }

            if (GlobalData.GameStarted == true) 
            {
                if (GlobalData.GameBid == 0) 
                {
                    this.viewModel.Status = "Set your bid";
                    return;
                }

                var httpClient = HttpClientService.GetInstance();
                LoginRequestDto loginData = new LoginRequestDto() 
                {
                    Login = GlobalData.UserData.Login,
                    Password = GlobalData.UserData.Password
                };

                GameResultDto result = await httpClient.PlayGameAsync(loginData, GlobalData.IsPass, GlobalData.GameBid, true);
                this.viewModel.Status = result.Result.ToString();


                if (result.Result.Contains("Parial win, you get"))
                {
                    this.viewModel.CurrentBalance = decimal.Round(HttpClientService.GetInstance().Login(new DTO.LoginRequestDto() { Login = GlobalData.UserData.Login, Password = GlobalData.UserData.Password }).Credits, 3).ToString();
                    this.ResetGame();
                    return;
                }

                if (result.Result.Contains("No win"))
                {
                    this.ResetGame();
                    return;
                }

                if (result.Result.Contains("wich is still less than hidden number. Continue playing")) 
                {
                    GlobalData.GameStarted = true;
                    return;
                }
                if (result.Result.Contains("You loose your card.")) 
                {
                    this.ResetGame();

                    if (MyCardsViewModel.Instance != null)
                    {
                        await MyCardsViewModel.Instance.RefreshCardsAsync();
                    }

                    if (PlayGameViewModel.Instance != null) 
                    {
                        await PlayGameViewModel.Instance.RefreshCardsAsync();
                    }

                    return;
                }
                if (result.Result.Contains("Congrats, you win! you get")) 
                {
                    this.viewModel.CurrentBalance = decimal.Round(HttpClientService.GetInstance().Login(new DTO.LoginRequestDto() { Login = GlobalData.UserData.Login, Password = GlobalData.UserData.Password }).Credits, 3).ToString();
                    this.ResetGame();

                    if (MyCardsViewModel.Instance != null)
                    {
                        await MyCardsViewModel.Instance.RefreshCardsAsync();
                        MyCardsViewModel.Instance.NewCards.Add(result.Rewards.RewardCard);
                    }

                    if (PlayGameViewModel.Instance != null)
                    {
                        await PlayGameViewModel.Instance.RefreshCardsAsync();
                    }

                    return;
                }

            }
        }
    }
}
