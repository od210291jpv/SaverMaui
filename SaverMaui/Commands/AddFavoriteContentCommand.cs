using Realms;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class AddFavoriteContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private FeedViewModel vievModel;

        public AddFavoriteContentCommand(FeedViewModel vm)
        {
            this.vievModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Realm _realm = Realm.GetInstance();
            var image = this.vievModel.CurrentContent;

            _realm.Write(() => image.IsFavorite = true);
        }
    }
}
