using BananasGambler.ViewModels;
using BananasGambler.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class NavigateToPackCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private PacksPageViewModel viewModel;

        public NavigateToPackCommand(PacksPageViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new CategoryFeedPage());
        }
    }
}
