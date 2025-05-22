using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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

        public async void Execute(object parameter)
        {
            this.viewModel.IsRefreshing = true;

            Realm _realm = Realm.GetInstance();
            var all = _realm.All<Content>().Where(c => c.Rating < 1).ToArray();

            if (all.Length == 0)
            {
                all = _realm.All<Content>().ToArray();
            }

            if (all.Length <= 1)
            {
                return;
            }

            var randomContent = all[new Random().Next(0, all.Length - 1)];
            if (FeedRandomContentViewModel.Instance != null)
            {
                FeedRandomContentViewModel.Instance.CurrentImage = new ImageRepresentationElement()
                {
                    CategoryId = randomContent.CategoryId ?? new Guid(),
                    Source = randomContent.ImageUri,
                    Name = randomContent.Title
                };

                var toast0 = Toast.Make($"Content category: {_realm.All<Category>().SingleOrDefault(c => c.CategoryId == randomContent.CategoryId)?.Name ?? "N/A"}", ToastDuration.Short, 14);
                await toast0.Show(new CancellationTokenSource().Token);
            }

            this.viewModel.IsRefreshing = false;
        }
    }
}
