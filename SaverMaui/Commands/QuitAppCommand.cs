using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class QuitAppCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.Quit();
        }
    }
}
