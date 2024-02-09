using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;
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
            string result = await Application.Current.MainPage.DisplayPromptAsync("Rate the content", "Please set from 1 to 5", "Ok", "Cancel");
            
            var isParsed = int.TryParse(result, out var parcedRate);

            if (!isParsed) 
            {
                await Application.Current.MainPage.DisplayAlert("Format error", "The value must be int", "Ok");
                return;
            }

            Realm _realm = Realm.GetInstance();
            var all = _realm.All<Content>().ToArray();

            var requiredContent = all.FirstOrDefault(c => c.ImageUri.Contains(this.feedViewModel.CurrentContent.Source.ToString().Replace("Uri: ", "")));

            _realm.Write(() => requiredContent.Rating = parcedRate);

            await Application.Current.MainPage.DisplayAlert("Done!", "Thank you for the rate!", "Ok");
        }
    }
}
