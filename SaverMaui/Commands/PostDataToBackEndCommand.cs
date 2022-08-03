using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts;
using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Content;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class PostDataToBackEndCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Realm realmInstance;

        private BackendServiceClient backendClient;

        public PostDataToBackEndCommand()
        {
            this.backendClient = BackendServiceClient.GetInstance();
        }

        public bool CanExecute(object parameter)
        {
            Realm _realm = Realm.GetInstance();

            if (_realm == null)
            {
                return false;
            }

            this.realmInstance = _realm;
            return true;
        }

        public async void Execute(object parameter)
        {
            var allContent = this.realmInstance.All<Content>().ToArray();
            var allCategories = this.realmInstance.All<Category>().ToArray();

            var allCategoriesDto = new List<CategoryDto>();
            var allContentDto = new List<ContentDto>();

            foreach (var cat in allCategories)
            {
                allCategoriesDto.Add(new CategoryDto()
                {
                    Name = cat.Name,
                    CategoryId = cat.CategoryId,
                });
            }

            foreach (var content in allContent) 
            {
                allContentDto.Add(new ContentDto() 
                {
                    CategoryId = content.CategoryId,
                    ImageUri = content.ImageUri,
                    Title = content.Title
                });
            }

            PostContentDataRequest request = new PostContentDataRequest()
            {
                Categories = allCategoriesDto.ToArray(),
                Content = allContentDto.ToArray()
            };

            var result = await this.backendClient.PostAllContentDataAsync(request);

            if (result == System.Net.HttpStatusCode.Created) 
            {
                await Application.Current.MainPage.DisplayAlert("Done", $"Categories added{allCategories.Length}, Content added {allContent.Length}", "Ok");
            }
            if (result == System.Net.HttpStatusCode.OK) 
            {
                await Application.Current.MainPage.DisplayAlert("Done", $"No new categories or content was found!", "Ok");
            }
        }
    }
}
