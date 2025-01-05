using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

using Realms;

using SaverMaui.Custom_Elements;
using SaverMaui.Models;
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

        Services.Contracts.Content.ContentDto[] allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentWithPaginationAsync(CurrentPage, 300);

        var ordered = allContent.OrderBy(i => i.Id).ToArray();

        if (allContent != null)
        {
            FeedViewModel.Instance.ContentCollection.Clear();

            foreach (var item in allContent)
            {
                FeedViewModel.Instance?.ContentCollection.Add(new ImageRepresentationElement()
                {
                    ContentId = item.Id,
                    Name = item.Title,
                    Source = item.ImageUri,
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
            Services.Contracts.Content.ContentDto[] allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentWithPaginationAsync(CurrentPage, 100);

            var ordered = allContent.OrderBy(i => i.Id).ToArray();

            if (allContent != null)
            {
                foreach (var item in ordered)
                {
                    FeedViewModel.Instance?.ContentCollection.Add(new ImageRepresentationElement()
                    {
                        ContentId = item.Id,
                        Name = item.Title,
                        Source = item.ImageUri,
                        CategoryId = item.CategoryId ?? new Guid()
                    });
                }
            }
        }

    }

    private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
	{
        Realm _realm = Realm.GetInstance();

		var all = _realm.All<Content>().ToArray();

		var content = all.Where(i => i.ImageUri.ToString().Contains(FeedViewModel.Instance.CurrentContent.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

        if (content == null) 
        {
            _realm.Add(new Content 
            {
                CategoryId = content.CategoryId,
                Id = content.Id,
                ImageUri = content.ImageUri,
                IsFavorite = content.IsFavorite,
                Title = content.Title
            });

            content = all.Where(i => i.ImageUri.ToString().Contains(FeedViewModel.Instance.CurrentContent.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();
        }

        _realm.Write(() => content.IsFavorite = true);
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make($"Content added to favorites", ToastDuration.Short, 14);
        await toast.Show(cancellationTokenSource.Token);
    }

    private void OnRateClicked(object sender, EventArgs e)
    {
        FeedViewModel.Instance?.RateImageCommand.Execute(FeedViewModel.Instance);
    }
}