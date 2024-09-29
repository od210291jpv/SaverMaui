using Realms;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class RandomContentRefreshCommand : ICommand
    {
        private FeedRandomContentViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public RandomContentRefreshCommand(FeedRandomContentViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.viewModel.IsRefreshing = true;

            Realm _realm = Realm.GetInstance();
            var all = _realm.All<Content>().ToArray();

            var randomContent = all[new Random().Next(0, all.Length - 1)];
            if (FeedRandomContentViewModel.Instance != null)
            {
                FeedRandomContentViewModel.Instance.CurrentImage = new ImageRepresentationElement()
                {
                    CategoryId = randomContent.CategoryId ?? new Guid(),
                    Source = randomContent.ImageUri,
                    Name = randomContent.Title
                };
            }

            this.viewModel.IsRefreshing = false;
        }
    }
}
