using SaverMaui.ViewModels;
using SaverMaui.Views;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class NavigateToWebContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private WebContentViewModel viewModel;

        public NavigateToWebContentCommand(WebContentViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            Environment.SahredData.currentWebContent = this.viewModel.SelectedWebContent;
            await Application.Current.MainPage.Navigation.PushAsync(new WebViewItemPage());
        }
    }
}
