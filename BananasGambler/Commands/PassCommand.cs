using BananasGambler.ViewModels;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class PassCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private PlayGameViewModel viewModel;

        public PassCommand(PlayGameViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (GlobalData.UserData.Login != "" && GlobalData.UserData.Password != "") 
            {
                return true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
            GlobalData.IsPass = true;
        }
    }
}
