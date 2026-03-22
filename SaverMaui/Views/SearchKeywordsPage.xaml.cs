
using Android.App.AppSearch;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using SaverMaui.Services;
using SaverMaui.Services.Contracts.Search;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class SearchKeywordsPage : ContentPage
{
	public SearchKeywordsPage()
	{
		InitializeComponent();
		this.Appearing += this.OnAppear;
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

        List<KeywordResult> searchResults = await BackendServiceClient.GetInstance().ContentActions.GetKeywordedSearchResults();

        if (BindingContext is SearchKeywordsViewModel viewModel) 
        {
            viewModel.SearchResults.Clear();
            foreach (var result in searchResults)
            {
                viewModel.SearchResults.Add(result);
            }
        }
    }

    private async void OnKeywordOpen(object sender, EventArgs e)
    {
        var swipeItem = (SwipeItem)sender;
        KeywordResult selectedKeyword = (KeywordResult)swipeItem.BindingContext;
        await Application.Current.MainPage.Navigation.PushAsync(new SearchResultsPage() { Keyword = selectedKeyword.Key });
    }
}