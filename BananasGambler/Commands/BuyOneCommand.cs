using BananasGambler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    internal class ByOneCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private PlayGameViewModel viewModel;

        public ByOneCommand(PlayGameViewModel vm)
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
