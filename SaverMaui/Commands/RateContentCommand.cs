using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;
using System.Threading;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class RateContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private FeedViewModel feedViewModel;

        public RateContentCommand(FeedViewModel vm)
        {
            this.feedViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            Realm _realm = Realm.GetInstance();
            var all = _realm.All<Content>().ToArray();

            var requiredContent = all.FirstOrDefault(c => c.ImageUri.Contains(this.feedViewModel.CurrentContent.Source.ToString().Replace("Uri: ", "")));

            var toast0 = Toast.Make($"Current content rate: {requiredContent.Rating}", ToastDuration.Short, 14);
            await toast0.Show(new CancellationTokenSource().Token);

            string result = await Application.Current.MainPage.DisplayPromptAsync("Rate the content", "Please set from 1 to 5", "Ok", "Cancel");

            if (result == null) 
            {
                return;
            }

            var isParsed = int.TryParse(result, out var parcedRate);

            if (!isParsed) 
            {
                await Application.Current.MainPage.DisplayAlert("Format error", "The value must be int", "Ok");
                return;
            }


            _realm.Write(() => requiredContent.Rating = parcedRate);

            var toast = Toast.Make($"Thank you for the rate!", ToastDuration.Short, 14);
            await toast.Show(new CancellationTokenSource().Token);
        }
    }
}
