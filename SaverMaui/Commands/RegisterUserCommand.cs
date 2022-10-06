using SaverMaui.Services.Helpers;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class RegisterUserCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return IsOnlineHelper.IsOnline;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
