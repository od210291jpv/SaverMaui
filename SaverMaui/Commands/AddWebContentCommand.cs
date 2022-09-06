using Realms;

using SaverMaui.Models;
using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class AddWebContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Realm realmInstance;

        private readonly WebContentViewModel vm;

        public AddWebContentCommand(WebContentViewModel viewModel)
        {
            this.vm = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            Realm _realm = Realm.GetInstance();

            if (_realm != null)
            {
                this.realmInstance = _realm;
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            this.realmInstance.Write(() =>
            this.realmInstance.Add<WebContent>(new WebContent()
            {
                Id = Guid.NewGuid(),
                Source = this.vm.ContentUrl,
                Title = this.vm.ContentTitle
            }));

            this.vm.ContentUrl = "";
            this.vm.RefreshContentOnPage();
        }
    }
}
