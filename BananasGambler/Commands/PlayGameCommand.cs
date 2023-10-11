using BananasGambler.DTO;
using BananasGambler.Services;
using BananasGambler.ViewModels;
using System.Linq;
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
            return true;
        }

        private void ResetGame() 
        {
            GlobalData.GameStarted = false;
            GlobalData.GameBid = 0;
            GlobalData.IsPass = false;
            this.viewModel.LockGameButtons(true);
            this.viewModel.BtnOneEnabled = true;
        }

        public async void Execute(object parameter)
        {
            this.viewModel.InitGameValues();

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
