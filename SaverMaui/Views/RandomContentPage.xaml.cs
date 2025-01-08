using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class RandomContentPage : ContentPage
{
    public RandomContentPage()
    {
        InitializeComponent();
        this.Appearing += OnPageAppearing;
    }

    private void OnPageAppearing(object sender, EventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        var all = _realm.All<Content>().Where(c => c.Rating < 1).ToArray();
        
        if (all.Length == 0) 
        {
            all = _realm.All<Content>().ToArray();
        }

        if (all.Length <= 1) 
        {
            return;
        }

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

    private async void OnRateClicked(object sender, EventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        var all = _realm.All<Content>().ToArray();

        var requiredContent = all.FirstOrDefault(c => c.ImageUri.Contains(FeedRandomContentViewModel.Instance.CurrentImage.Source.ToString().Replace("Uri: ", "")));

        var toast0 = Toast.Make($"Current content rate: {requiredContent.Rating}", ToastDuration.Short, 14);
        await toast0.Show(new CancellationTokenSource().Token);

        string result = await Application.Current.MainPage.DisplayPromptAsync("Rate the content", "Please set from 1 to 5", "Ok", "Cancel");

        if (result == null)
        {
            return;
        }

        var isParsed = int.TryParse(result, out var parcedRate);

        if (!isParsed)
        {
            await Application.Current.MainPage.DisplayAlert("Format error", "The value must be int", "Ok");
            return;
        }


        _realm.Write(() => requiredContent.Rating = parcedRate);

        var toast = Toast.Make($"Thank you for the rate!", ToastDuration.Short, 14);
        await toast.Show(new CancellationTokenSource().Token);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (FeedRandomContentViewModel.Instance is null) 
        {
            return;
        }

        Realm _realm = Realm.GetInstance();
        var all = _realm.All<Content>().ToArray();

        Content img = all.Where(i => i.ImageUri.ToString().Contains(FeedRandomContentViewModel.Instance.CurrentImage.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

        var response = await BackendServiceClient.GetInstance().ContentActions.DeleteContentAsync(img.Id);

        if (img != null)
        {
            _realm.Write(() => _realm.Remove(img));
        }

        new RandomContentRefreshCommand(FeedRandomContentViewModel.Instance).Execute(null);

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var toast = Toast.Make($"Content was removed", ToastDuration.Short, 14);
        await toast.Show(cancellationTokenSource.Token);
    }
}
