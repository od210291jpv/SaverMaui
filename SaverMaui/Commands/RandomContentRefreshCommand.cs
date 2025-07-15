using SaverMaui.Custom_Elements;
using SaverMaui.Services;
using SaverMaui.Services.Contracts.Content;
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

            ContentDto randomContent = await BackendServiceClient.GetInstance().ContentActions.GetRandomContent();
            if (FeedRandomContentViewModel.Instance != null)
            {
                FeedRandomContentViewModel.Instance.CurrentImage = new ImageRepresentationElement()
                {
                    CategoryId = randomContent.CategoryId ?? new Guid(),
                    Source = randomContent.ImageUri,
                    Name = randomContent.Title,
                    ContentId = randomContent.Id
                };
            }

            this.viewModel.IsRefreshing = false;
        }
    }
}
