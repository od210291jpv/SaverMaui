using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace SaverMaui.Views;

public partial class RandomContentPage : ContentPage
{
    public RandomContentPage()
    {
        InitializeComponent();
        this.Appearing += OnPageAppearing;
    }

    private async void OnPageAppearing(object sender, EventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        var all = _realm.All<Content>().ToArray();

        var randomContent = all[new Random().Next(0, all.Length - 1)];
        if (FeedRandomContentViewModel.Instance != null) 
        {
            FeedRandomContentViewModel.Instance.CurrentImage = new ImageRepresentationElement()
            {
                CategoryId = randomContent.CategoryId ?? new Guid(),
                Source = randomContent.ImageUri,
                Name = randomContent.Title
            };
        }
    }

    private async void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        var all = _realm.All<Content>().ToArray();

        Content feed = all.Where(i => i.ImageUri.ToString().Contains(FeedRandomContentViewModel.Instance.CurrentImage.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

        _realm.Write(() => feed.IsFavorite = true);
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make($"Content added to favorites", ToastDuration.Short, 14);
        await toast.Show(cancellationTokenSource.Token);
    }
}
