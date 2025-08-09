using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

using SaverMaui.Custom_Elements;
using SaverMaui.Services;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;
public partial class FeedPage : ContentPage
{
    private static short CurrentPage = 0;
    private static bool InitialLoad = true;

    public FeedPage()
	{
		InitializeComponent();
        this.Appearing += OnFeedAppearing;
    }

    private async void OnFeedAppearing(object sender, EventArgs e)
    {
        if (Environment.Login == null || Environment.Password == null) 
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"Only authorized users can access Live Feed", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);

            return;
        }

        var allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentWithPaginationAsync(CurrentPage, 50);

        if (allContent != null)
        {
            FeedViewModel.Instance.ContentCollection.Clear();

            foreach (var item in allContent)
            {
                FeedViewModel.Instance?.ContentCollection.Add(new ImageRepresentationElement()
                {
                    ContentId = item.Id,
                    Name = item.Title,
                    Source = new UriImageSource()
                    {
                        Uri = new Uri(item.ImageUri),
                        CachingEnabled = true,
                        CacheValidity = TimeSpan.FromDays(30)
                    },
                    CategoryId = item.CategoryId ?? new Guid()
                });
            }

            InitialLoad = false;
        }
    }

    public async void OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {

        if (InitialLoad == true)
        {
            return;
        }

        var currentItem = (ImageRepresentationElement)e.CurrentItem;
        if (currentItem.Id == FeedViewModel.Instance?.ContentCollection.Last().Id)
        {
            CurrentPage += 1;
            Services.Contracts.Content.ContentDto[] allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentWithPaginationAsync(CurrentPage, 50);

            if (allContent != null)
            {
                foreach (var item in allContent)
                {
                    FeedViewModel.Instance?.ContentCollection.Add(new ImageRepresentationElement()
                    {
                        ContentId = item.Id,
                        Name = item.Title,
                        Source = new UriImageSource()
                        {
                            Uri = new Uri(item.ImageUri),
                            CachingEnabled = true,
                            CacheValidity = TimeSpan.FromDays(30)
                        },
                        CategoryId = item.CategoryId ?? new Guid()
                    });
                }
            }
        }
    }

    private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
	{
        if (Environment.IsLoggedIn == true) 
        {
            var result = await BackendServiceClient.GetInstance().ContentActions.BuyContent(Environment.ProfileIntId, FeedViewModel.Instance.CurrentContent.ContentId);

            if (result != System.Net.HttpStatusCode.OK) 
            {
                CancellationTokenSource tok = new CancellationTokenSource();
                var toast2 = Toast.Make($"Looks like no enough funds. Status is {result}", ToastDuration.Short, 14);
                await toast2.Show(tok.Token);

                return;
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"Content added to favorites", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);

            return;
        }

        CancellationTokenSource c = new CancellationTokenSource();
        var t = Toast.Make($"Please login to be able to purchase content", ToastDuration.Short, 14);
        await t.Show(c.Token);
    }

    private void OnRateClicked(object sender, EventArgs e)
    {
        FeedViewModel.Instance?.RateImageCommand.Execute(FeedViewModel.Instance);
    }

}