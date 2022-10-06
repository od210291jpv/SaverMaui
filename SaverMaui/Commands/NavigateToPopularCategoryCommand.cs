using SaverMaui.ViewModels;
using SaverMaui.Views;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class NavigateToPopularCategoryCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private MostPopularCategoriesViewModel viewModel;

        public NavigateToPopularCategoryCommand(MostPopularCategoriesViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            Environment.SahredData.currentCategory = this.viewModel.SelectedCategory;
            await Application.Current.MainPage.Navigation.PushAsync(new CategoryFeedPage());
        }
    }
}
