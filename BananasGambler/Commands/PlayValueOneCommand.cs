using BananasGambler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class PlayValueOneCommand : ICommand
    {
        private PlayGameViewModel viewModel;

        public PlayValueOneCommand(PlayGameViewModel vm)
        {
            this.viewModel = vm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            viewModel.ValueToPlay = int.Parse(viewModel.ValueOne);
            GlobalData.GameBid += viewModel.ValueToPlay;
            this.viewModel.BtnOneEnabled = false;
        }
    }
}
