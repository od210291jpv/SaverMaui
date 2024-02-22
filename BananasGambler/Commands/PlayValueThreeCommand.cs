using BananasGambler.ViewModels;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class PlayValueThreeCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private PlayGameViewModel viewModel;

        public PlayValueThreeCommand(PlayGameViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.viewModel.ValueToPlay = int.Parse(viewModel.ValueThree);
            GlobalData.GameBid += viewModel.ValueToPlay;
            this.viewModel.BtnThreeEnabled = true;
        }
    }
}
