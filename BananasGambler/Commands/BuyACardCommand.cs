using BananasGambler.Services;
using BananasGambler.ViewModels;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class BuyACardCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ProfileViewModel viewModel;

        public BuyACardCommand(ProfileViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var client = HttpClientService.GetInstance();
            DTO.LoginResponseDto clientData = await client.LoginAsync(new DTO.LoginRequestDto() { Login = GlobalData.UserData.Login, Password = GlobalData.UserData.Password });
            this.viewModel.Credits = decimal.Round(clientData.Credits, 2) - 1m;

            var allAvailableCards = await client.GetAllCardsAsync();
            var allPlayerCards = await client.GetUserCardsAsync(new DTO.LoginRequestDto() { Login = GlobalData.UserData.Login, Password = GlobalData.UserData.Password });

            var playerCardsIds = allPlayerCards.Select(x => x.Id).ToArray();
            var allCardsIds = allAvailableCards.Where(c => c.CostInCredits <= clientData.Credits).Where(x => playerCardsIds.Contains(x.Id) == false).Select(x => x.Id).ToArray();
            var randomCard = new Random().Next(allCardsIds.First(), allCardsIds.Last());

            clientData = await client.LoginAsync(new DTO.LoginRequestDto() { Login = GlobalData.UserData.Login, Password = GlobalData.UserData.Password });

            System.Net.HttpStatusCode result = await client.BuyACardAsync(new DTO.LoginRequestDto() { Login = GlobalData.UserData.Login, Password = GlobalData.UserData.Password }, randomCard);
            if (result == System.Net.HttpStatusCode.OK)
            {
                this.viewModel.Credits = decimal.Round(clientData.Credits, 2);

                if (MyCardsViewModel.Instance != null) 
                {
                    await MyCardsViewModel.Instance.RefreshCardsAsync();
                    MyCardsViewModel.Instance.NewCards.Add(randomCard);
                }                
                await Application.Current.MainPage.DisplayAlert("Congrats", $"You owned a card: {randomCard}", "Great!");
            }
            else 
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Something went wrong", "OK");
            }
        }
    }
}
