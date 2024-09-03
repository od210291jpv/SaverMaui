using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
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

        GetAllContentResponseModel[] allContent = await BackendServiceClient.GetInstance().GetAllContentAsync();

        var feeddata = new ObservableCollection<ImageRepresentationElement>();

        if (FeedViewModel.Instance?.ContentCollection != null) 
        {
            foreach (var cont in allContent.OrderBy(i => i.Id)) 
            {
                feeddata.Add(new ImageRepresentationElement 
                {
                    CategoryId = cont.CategoryId,
                    Name = cont.Title,
                    Source = cont.ImageUri,
                });
            }

            FeedViewModel.Instance.ContentCollection = feeddata;
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