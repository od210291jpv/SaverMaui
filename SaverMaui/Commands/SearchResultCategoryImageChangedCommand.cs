using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class SearchResultCategoryImageChangedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private SearchCategoryFeedViewModel viewModel;

        public SearchResultCategoryImageChangedCommand(SearchCategoryFeedViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //Environment.CurrectSearchResultItem = SearchCategoryFeedViewModel.instance.CurrentResult.Url;
        }
    }
}
