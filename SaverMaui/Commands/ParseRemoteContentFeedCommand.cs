using Realms;

using SaverMaui.Models;
using SaverMaui.Services.FeedParser;
using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class ParseRemoteContentFeedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private SettingsViewModel viewModel;

        public ParseRemoteContentFeedCommand(SettingsViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            FeedParserService feedParser = new FeedParserService(new Uri(this.viewModel.FeedUrl));
            await feedParser.LoadFeedByUrl();

            var images = feedParser.GetFeedImages();
            var categoryName = feedParser.GetFeedTitle();

            if (images.Count == 0) 
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No images found in feed", "Ok");
            }

            Realm realmInstance = Realm.GetInstance();

            Category category = new Category()
            {
                CategoryId = Guid.NewGuid(),
                Name = categoryName,
                IsFavorite = false,
            };

            realmInstance.Write(() => realmInstance.Add(category));

            CategoriesViewModel.Instance?.UpdateAllCategories();
            SettingsViewModel.GetInstance().UpdateAllCategories();

            SettingsViewModel.GetInstance().CategoriesAmount += 1;

            foreach (var img in images) 
            {
                Content content = new Content()
                {
                    CategoryId = category.CategoryId,
                    ImageUri = img,
                    Title = $"Parsed {DateTime.Now.Day}"
                };

                realmInstance.Write(() => realmInstance.Add<Content>(content));
                SettingsViewModel.GetInstance().ContentAmount += 1;
            }

            await Application.Current.MainPage.DisplayAlert("Done", $"New Content Added! {images.Count}", "Ok");
        }
    }
}
