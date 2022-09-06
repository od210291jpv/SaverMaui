using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class SyncAllContentCommand : ICommand
    {
        private SettingsViewModel _vm;

        private Realm realmInstance;

        private BackendServiceClient backendClient;

        private ActivityIndicator activity = new ActivityIndicator() { IsRunning = false, IsVisible = false };

        public SyncAllContentCommand(SettingsViewModel vm)
        {
            this._vm = vm;

            this.backendClient = BackendServiceClient.GetInstance();
        }

        public event EventHandler CanExecuteChanged;

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

        public async void Execute(object parameter)
        {
            this.activity.IsVisible = true;
            this.activity.IsRunning = true;

            var allCategories = await this.backendClient.GetAllCategoriesAsync();
            var allContent = await this.backendClient.GetAllContentAsync();

            foreach (var cat in allCategories) 
            {
                Category category = new Category()
                {
                    CategoryId = cat.CategoryId,
                    Name = cat.Name,
                    IsFavorite = false,

                };

                if (cat.AmountOfOpenings != null)
                {
                    category.AmountOfOpenings = cat.AmountOfOpenings.Value;
                }
                else 
                {
                    category.AmountOfOpenings = 0;
                }

                if (cat.AmountOfFavorites != null)
                {
                    category.AmountOfFavorites = cat.AmountOfFavorites.Value;
                }
                else 
                {
                    cat.AmountOfFavorites = 0;
                }

                realmInstance.Write(() => realmInstance.Add<Category>(category));
            }

            int syncedContent = 0;

            foreach (var content in allContent) 
            {
                if (realmInstance.All<Content>().Where(ct => ct.ImageUri == content.ImageUri).ToArray().Length == 0) 
                {
                    Content ct = new Content()
                    {
                        CategoryId = content.CategoryId,
                        ImageUri = content.ImageUri.Replace(" ", ""),
                        IsFavorite = false,
                        Title = content.Title
                    };

                    realmInstance.Write(() => realmInstance.Add<Content>(ct));
                    syncedContent++;
                }
            }

            this.activity.IsVisible = false;
            this.activity.IsRunning = false;

            await Application.Current.MainPage.DisplayAlert("Done", $"Categories added{allCategories.Length}, Content added {syncedContent}", "Ok");
        }
    }
}
