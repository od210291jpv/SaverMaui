using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class CategoryFeedCarouselItemChangedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private CategoryFeedViewModel viewModel;

        public CategoryFeedCarouselItemChangedCommand(CategoryFeedViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return NotificationCenterViewModel.GetInstance().IsConnected == true && SettingsViewModel.GetInstance().LogFeedsEnabled == true;
        }

        public async void Execute(object parameter)
        {
            try
            {


                NotificationCenterViewModel.GetInstance()?.SendLocalMessage($"Debug: " +
                    $"loading image {this.viewModel.CurrentImage.Id} \r\n" +
                    $"Is loaded {this.viewModel.CurrentImage.IsLoaded} \r\n" +
                    $"Is visible {this.viewModel.CurrentImage.IsVisible}\r\n" +
                    $"Image name {this.viewModel.CurrentImage.Name}\r\n");
            }
            catch (Exception) 
            {
            }
        }

        private async Task<string> GetImageCategory(Guid imageId) 
        {
            var realm = await Realm.GetInstanceAsync();
            var result = realm.All<Category>().ToArray().Single(ct => ct.CategoryId == imageId);
            return result?.Name;
        }
    }
}
