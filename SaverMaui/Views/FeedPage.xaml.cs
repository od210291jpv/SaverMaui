using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;
using System.Collections.ObjectModel;

namespace SaverMaui.Views;
public partial class FeedPage : ContentPage
{
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

        var notSortedAllContent = await BackendServiceClient.GetInstance().GetAllContentAsync();
        GetAllContentResponseModel[] allContent = notSortedAllContent.OrderBy(c => c.DateCreated).ToArray();

        var feeddata = new ObservableCollection<ImageRepresentationElement>();

        if (FeedViewModel.Instance?.ContentCollection != null) 
        {
            foreach (var cont in allContent) 
            {
                feeddata.Add(new ImageRepresentationElement 
                {
                    CategoryId = cont.CategoryId,
                    Name = cont.Title,
                    Source = cont.ImageUri,
                    ContentId = cont.Id
                });
            }

            if (FeedViewModel.Instance.ContentCollection.Count == 0) 
            {
                FeedViewModel.Instance.ContentCollection = feeddata;
                return;
            }

            var con1 = FeedViewModel.Instance.ContentCollection.AsParallel().Select(i => i.ContentId).ToArray();
            var con2 = feeddata.AsParallel().Select(i => i.ContentId).ToArray();

            var nn = con1.Except(con2).ToArray();

            if (nn.Length > 0) 
            {
                foreach (var i in nn.Order())
                {
                    FeedViewModel.Instance.ContentCollection.Add(feeddata.Single(c => c.ContentId == i));
                }

            }
        }
    }

    private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
	{
        Realm _realm = Realm.GetInstance();
		var all = _realm.All<Content>().ToArray();

		var content = all.Where(i => i.ImageUri.ToString().Contains(Environment.CurrentImageOnScreen.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

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

            content = all.Where(i => i.ImageUri.ToString().Contains(Environment.CurrentImageOnScreen.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();
        }

        _realm.Write(() => content.IsFavorite = true);
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make($"Content added to favorites", ToastDuration.Short, 14);
        await toast.Show(cancellationTokenSource.Token);
    }

	private void OnPinchGestureTapped(object sender, EventArgs e) 
	{
    }

    private void OnRateClicked(object sender, EventArgs e)
    {
        FeedViewModel.Instance?.RateImageCommand.Execute(FeedViewModel.Instance);
    }
}