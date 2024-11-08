using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class SearchCurrentItemChangedCommand : ICommand
    {
        private SearchResultsViewModel vm;

        public event EventHandler CanExecuteChanged;

        public SearchCurrentItemChangedCommand(SearchResultsViewModel vm)
        {
            this.vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Environment.CurrectSearchResultCategory = vm.CurrentCategory;
        }
    }
}
