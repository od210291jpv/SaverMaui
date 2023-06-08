using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts;
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

            Content[] allLocalContent = this.realmInstance.All<Content>().ToArray();
            var allLocalCategories = this.realmInstance.All<Category>().ToArray();

            GetAllCategoriesResponseModel[] allCategories = await this.backendClient.GetAllCategoriesAsync();

            var allLocalCategorioesIds = allLocalCategories
            .Select(lc => lc.CategoryId)
            .ToArray();

            var catsToBeAdded = allCategories.Where(ct =>
            allLocalCategorioesIds
            .Contains(ct.CategoryId) == false).ToArray();

            GetAllContentResponseModel[] allContent = await this.backendClient.GetAllContentAsync();

            GetAllContentResponseModel[] contentToBeAdded = CalculateContentToBeAdded(allContent, allLocalContent);

            foreach (var cat in catsToBeAdded) 
            {
                Category category = new Category()
                {
                    CategoryId = cat.CategoryId,
                    Name = cat.Name,
                    IsFavorite = false,
                    AmountOfOpenings = cat.AmountOfOpenings != null ? cat.AmountOfOpenings.Value : 0,
                    AmountOfFavorites = cat.AmountOfFavorites != null ? cat.AmountOfFavorites.Value : 0,
                };

                realmInstance.Write(() => realmInstance.Add<Category>(category));
            }

            int syncedContent = 0;

            foreach (var content in contentToBeAdded) 
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

            await Application.Current.MainPage.DisplayAlert("Done", $"Categories added{catsToBeAdded.Length}, Content added {contentToBeAdded.Length}", "Ok");
        }

        private GetAllContentResponseModel[] CalculateContentToBeAdded(GetAllContentResponseModel[] allContent, Content[] allLocalContent) 
        {
            var allLocalContentIds = allLocalContent
                .Select(lcn => lcn.ImageUri).ToArray();

            return allContent.Where(cn =>
                allLocalContentIds
                .Contains(cn.ImageUri) == false).ToArray();
        }
    }
}
