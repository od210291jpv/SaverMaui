
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts;
using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Content;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;

namespace SaverMaui.Views
{
    public partial class SearchResultsPage : ContentPage
    {
        public SearchResultsPage()
        {
            InitializeComponent();
            this.Appearing += OnAppear;
        }

        private async void OnAppear(object sender, EventArgs e)
        {
            var searchResults = await BackendServiceClient.GetInstance().ContentActions.GetSearchResults();

            Realm _realm = Realm.GetInstance();

            var cats = _realm.All<Category>().ToArray();
            var reqCat = cats.FirstOrDefault(c => c.Name == "Rest");

            var existingContent = _realm.All<Content>().Where(c => c.CategoryId == reqCat.CategoryId).ToArray().Select(c => c.ImageUri).ToArray();

            var resultsSorted = searchResults.AsParallel().Where(c => existingContent.Contains(c.Replace("Uri: ", "")) == false).Order().ToArray();

            if (SearchResultsViewModel.Instance?.ContentCollection != null) 
            {
                SearchResultsViewModel.Instance.ClearContent();
            }

            foreach (var searchResult in resultsSorted)
            {
                SearchResultsViewModel.Instance?.ContentCollection.Add(new Custom_Elements.SearchResult() { Url = searchResult });
            }
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var toast = Toast.Make($"Content found: {searchResults.Length}", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
        }

        private async void OnRateClicked(object sender, EventArgs e)
        {
            Realm _realm = Realm.GetInstance();

            var cats = _realm.All<Category>().ToArray();
            var reqCat = cats.FirstOrDefault(c => c.Name == "Rest");

            if (reqCat != null)
            {
                Content content = new Content()
                {
                    CategoryId = reqCat.CategoryId,
                    ImageUri = Environment.CurrectSearchResultItem,
                    Title = "Sexy"
                };

                _realm.Write(() => _realm.Add<Content>(content));

                var backendClient = BackendServiceClient.GetInstance();

                PostContentDataRequest request = new PostContentDataRequest()
                {
                    Categories = Array.Empty<CategoryDto>(),
                    Content = new ContentDto[] { new ContentDto
                    {
                        CategoryId = reqCat.CategoryId,
                        DateCreated = DateTime.Now,
                        Title = "Sexy",
                        ImageUri = Environment.CurrectSearchResultItem
                    }}
                };

                await backendClient.PostAllContentDataAsync(request);

                await Application.Current.MainPage.DisplayAlert("Ok", $"Content added", "Ok");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Required category  does not exist!", "Ok");
            }
        }
    }
}