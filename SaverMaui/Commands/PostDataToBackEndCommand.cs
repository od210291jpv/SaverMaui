using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts;
using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Content;
using SaverMaui.Services.ServiceExtensions;
using System.Net;
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
            var allContent = this.realmInstance.All<Content>().Where(c => c.ImageUri != "" && c.ImageUri != null).ToArray();
            var allCategories = this.realmInstance.All<Category>().ToArray();

            var allCategoriesDto = new List<CategoryDto>();
            var allContentDto = new List<ContentDto>();

            var allExistingsCategories = await this.backendClient.GetAllCategoriesAsync();

            //foreach (var cat in allCategories)
            //{
            //    if (!allExistingsCategories.Select(c => c.CategoryId).ToArray().Contains(cat.CategoryId)) 
            //    {
            //        allCategoriesDto.Add(new CategoryDto()
            //        {
            //            Name = cat.Name,
            //            CategoryId = cat.CategoryId,
            //            AmountOfFavorites = cat.AmountOfFavorites,
            //            AmountOfOpenings = cat.AmountOfOpenings,
            //            PublisherProfileId = Environment.ProfileId
            //        });
            //    }
            //}

            var allExistingContent = await this.backendClient.GetAllContentAsync();

            //foreach (var content in allContent) 
            //{
            //    if (!allExistingContent.Select(ct => ct.ImageUri).ToArray().Contains(content.ImageUri)) 
            //    {
            //        allContentDto.Add(new ContentDto()
            //        {
            //            CategoryId = content.CategoryId,
            //            ImageUri = content.ImageUri,
            //            Title = content.Title
            //        });
            //    }
            //}

            var contentAmt = allContent.Length;

            for (int i = 0; i <= contentAmt - 1; i++) 
            {
                if (!allExistingContent.Select(ct => ct.ImageUri).ToArray().Contains(allContent[i].ImageUri))
                {
                    allContentDto.Add(new ContentDto()
                    {
                        CategoryId = allContent[i].CategoryId,
                        ImageUri = allContent[i].ImageUri,
                        Title = allContent[i].Title,
                        Rating = (short)allContent[i].Rating,
                    });
                }
            }

            PostContentDataRequest postCategoriesRequest = new PostContentDataRequest()
            {
                Categories = allCategoriesDto.ToArray(),
                Content = Array.Empty<ContentDto>(),
            };

            var result = await this.backendClient.PostAllContentDataAsync(postCategoriesRequest);

            int contentAmount = allContentDto.Count();
            int skip = 0;
            int take = 1000;


            while (skip < contentAmount) 
            {
                PostContentDataRequest request = new PostContentDataRequest()
                {
                    Categories = Array.Empty<CategoryDto>(),
                    Content = allContentDto.Skip(skip).Take(take).ToArray()
                };

                result = await this.backendClient.PostAllContentDataAsync(request);
                skip += 1000;

                CancellationTokenSource cancellationTokenSource2 = new CancellationTokenSource();
                var toast2 = Toast.Make($"Syncing content: {skip}", ToastDuration.Short, 14);
                await toast2.Show(cancellationTokenSource2.Token);

                if (result != System.Net.HttpStatusCode.Created && result == System.Net.HttpStatusCode.OK)
                {
                    toast2 = Toast.Make($"Error occured!", ToastDuration.Short, 14);
                    await toast2.Show(cancellationTokenSource2.Token);
                }
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"Content syncronization finished", ToastDuration.Long, 14);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
