using SaverMaui.ViewModels;
using SaverMaui.Views;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class NavigateToSearchResultsFeedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private SearchResultsViewModel viewModel;

        public NavigateToSearchResultsFeedCommand(SearchResultsViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public async void Execute(object parameter)
        {
            Environment.CurrectSearchResultCategory = this.viewModel.CurrentCategory;

            await Application.Current.MainPage.Navigation.PushAsync(new SearchCategoryFeedPage());
        }
    }
}
