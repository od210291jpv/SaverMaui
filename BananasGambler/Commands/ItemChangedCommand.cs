using BananasGambler.ViewModels;
using System.Windows.Input;

namespace BananasGambler.Commands
{
    class ItemChangedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MyCardsViewModel viewModel;

        public ItemChangedCommand(MyCardsViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (GlobalData.UserData.Login == "" | GlobalData.UserData.Password == "")
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            if (this.viewModel.CurrentCard == null) 
            {
                return;
            }

            if (this.viewModel.NewCards.Contains(this.viewModel.CurrentCard.Id) == true)
            {
                
                var card = this.viewModel.Cards.Single(c => c.Id == this.viewModel.CurrentCard.Id);
                card.IsNewItem = true;
                return;
            }
        }
    }
}
