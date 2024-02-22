using BananasGambler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class PlayValueTwoCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private PlayGameViewModel viewModel;

        public PlayValueTwoCommand(PlayGameViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.viewModel.ValueToPlay = int.Parse(viewModel.ValueTwo);
            GlobalData.GameBid += viewModel.ValueToPlay;
            this.viewModel.BtnTwoEnabled = true;
        }
    }
}
