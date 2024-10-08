using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts;
using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Content;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
            string[] searchResults = await BackendServiceClient.GetInstance().ContentActions.GetSearchResults();

            ObservableCollection<KeyValuePair<string, SearchResult[]>> grouppedresults = this.GetGrouppedSearchResults(searchResults);

            Realm _realm = Realm.GetInstance();

            var cats = _realm.All<Category>().ToArray();
            var reqCat = cats.FirstOrDefault(c => c.Name == "Rest");

            if (SearchResultsViewModel.Instance?.ContentCollection != null) 
            {
                SearchResultsViewModel.Instance.ClearContent();
            }

            foreach (var g in grouppedresults) 
            {
                SearchResultsViewModel.Instance?.ContentCollection.Add(g);
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var toast = Toast.Make($"Content found: {searchResults.Length}", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
        }

        private async void OnRateClicked(object sender, EventArgs e)
        {

        }
        private ICommand navigateToSearchCategoryFeedCommand;

        public ICommand NavigateToSearchCategoryFeedCommand 
        {
            get 
            {
                return this.navigateToSearchCategoryFeedCommand ?? (new NavigateToSearchResultsFeedCommand(SearchResultsViewModel.Instance));
            } 
        }

        private ObservableCollection<System.Collections.Generic.KeyValuePair<string, SearchResult[]>> GetGrouppedSearchResults(string[] results)
        {
            var fullResults = new ObservableCollection<System.Collections.Generic.KeyValuePair<string, SearchResult[]>>();
            var res = results.GroupBy((s) => s.Split("/").Last().Split("_").First()).ToArray();

            foreach (var r in res) 
            {
                fullResults.Add(new System.Collections.Generic.KeyValuePair<string, SearchResult[]>(r.Key, r.Select(i => new SearchResult() { Name = r.Key, Url = i }).ToArray()));
            }

            return fullResults;
        }

        private void OnCategoryOpen(object sender, EventArgs e)
        {
            this.NavigateToSearchCategoryFeedCommand.Execute(SearchResultsViewModel.Instance);
        }
    }
}