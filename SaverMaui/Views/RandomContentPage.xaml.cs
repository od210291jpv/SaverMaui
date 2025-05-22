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

    private async void OnPageAppearing(object sender, EventArgs e)
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

            var toast0 = Toast.Make($"Content category: {_realm.All<Category>().SingleOrDefault(c => c.CategoryId == randomContent.CategoryId)?.Name ?? "N/A"}", ToastDuration.Short, 14);
            await toast0.Show(new CancellationTokenSource().Token);
        }
    }

    private async void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        var all = _realm.All<Content>().ToArray();

        Content expectedContent = all.Where(i => i.ImageUri.ToString().Contains(FeedRandomContentViewModel.Instance.CurrentImage.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

        if (Environment.IsLoggedIn == true) 
        {
            var result = await BackendServiceClient.GetInstance().ContentActions.BuyContent(Environment.ProfileIntId, expectedContent.Id);

            if (result != System.Net.HttpStatusCode.OK)
            {
                CancellationTokenSource tok = new CancellationTokenSource();
                var toast2 = Toast.Make($"Looks like no enough funds. Status is {result}", ToastDuration.Short, 14);
                await toast2.Show(tok.Token);

                return;
            }
        }

        _realm.Write(() => expectedContent.IsFavorite = true);
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

        if (Environment.ProfileIntId != 0) 
        {
            var resp = await BackendServiceClient.GetInstance().ContentActions.RateContent(requiredContent.Id, Environment.ProfileIntId, (short)requiredContent.Rating);
        }

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
