using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Views
{
    public partial class SearchResultsPage : ContentPage
    {
        public string Keyword { get; set; } = string.Empty;

        public SearchResultsPage()
        {
            InitializeComponent();
            this.Appearing += OnAppear;
        }

        private async void OnAppear(object sender, EventArgs e)
        {
            if (Environment.Login == null | Environment.Login == string.Empty) 
            {
                CancellationTokenSource cts = new CancellationTokenSource();

                var tst = Toast.Make($"Please login to review the search results", ToastDuration.Short, 14);
                await tst.Show(cts.Token);
            }

            if (Environment.SearchResultsResfresh == false)
            {
                return;
            }

            var vm = SearchKeywordsViewModel.Instance;

            var searchResults = vm?.SearchResults.Single(i => i.Key == this.Keyword).Urls.ToArray();

            var sotredGroupped = this.GetGrouppedSearchResults(searchResults).OrderBy(g => g.Key);

            if (SearchResultsViewModel.Instance?.ContentCollection != null)
            {
                SearchResultsViewModel.Instance.CurrentKeyword = 0;
                SearchResultsViewModel.Instance.ClearContent();
            }

            foreach (KeyValuePair<string, SearchResult[]> g in sotredGroupped) 
            {
                SearchResultsViewModel.Instance?.ContentCollection.Add(g);
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var toast = Toast.Make($"Content found: {searchResults.Length}", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
        }

        private ICommand navigateToSearchCategoryFeedCommand;

        public ICommand NavigateToSearchCategoryFeedCommand 
        {
            get 
            {
                return this.navigateToSearchCategoryFeedCommand ?? (new NavigateToSearchResultsFeedCommand(SearchResultsViewModel.Instance));
            } 
        }

        private List<KeyValuePair<string, SearchResult[]>> GetGrouppedSearchResults(string[] results)
        {
            var fullResults = new List<KeyValuePair<string, SearchResult[]>>();
            var res = results.GroupBy((s) => s.Split("/").Last().Split("_").First()).ToArray();

            foreach (var r in res) 
            {
                fullResults.Add(new KeyValuePair<string, SearchResult[]>(r.Key, r.Select(i => new SearchResult() { Name = r.Key, Url = i }).ToArray()));
            }
            return fullResults;
        }

        private void OnCategoryOpen(object sender, TappedEventArgs e)
        {
            this.NavigateToSearchCategoryFeedCommand.Execute(SearchResultsViewModel.Instance);
        }
    }
}