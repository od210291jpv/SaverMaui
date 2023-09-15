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
            if (GlobalData.GameStarted == false) 
            {
                return false;
            }

            return true;
        }

        public async void Execute(object parameter)
        {
            viewModel.ValueToPlay = int.Parse(viewModel.ValueOne);
        }
    }
}
