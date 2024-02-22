using BananasGambler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class BuySevenCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private PlayGameViewModel viewModel;

        public BuySevenCommand(PlayGameViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
