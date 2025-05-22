using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class FavoriteContentPage : ContentPage
{
	public FavoriteContentPage()
	{
		InitializeComponent();
        this.Appearing += OnAppearing;
	}

    private async void OnAppearing(object sender, EventArgs e)
    {
        if (Environment.IsLoggedIn == false) 
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"You are offline, local data will be shown", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);

            Realm _realm = Realm.GetInstance();
            Content[] allRelatedContent = _realm.All<Content>().Where(ct => ct.IsFavorite == true).ToArray();

            if (FavoriteContentViewModel.Instance != null)
            {
                foreach (Content cat in allRelatedContent)
                {
                    FavoriteContentViewModel.Instance.ContentCollection.Add(new ImageRepresentationElement()
                    {
                        CategoryId = cat.CategoryId.Value,
                        Name = cat.Title,
                        Source = cat.ImageUri,
                        IsFavorite = cat.IsFavorite
                    });
                }
            }

            return;
        }

        var allCOntentIds = await BackendServiceClient.GetInstance().ContentActions.GetFavoriteContent(Environment.Login, Environment.Password);
        var allContent = await BackendServiceClient.GetInstance().ContentActions.GetContentById(allCOntentIds);

        if (FavoriteContentViewModel.Instance != null)
        {
            foreach (var cat in allContent)
            {
                FavoriteContentViewModel.Instance.ContentCollection.Add(new ImageRepresentationElement()
                {
                    CategoryId = cat.CategoryId.Value,
                    Name = cat.Title,
                    Source = cat.ImageUri,
                    IsFavorite = true,
                    Rating = cat.Rating
                });
            }
        }

        return;
    }

    private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
	{
        Realm _realm = Realm.GetInstance();
        var all = _realm.All<Content>().ToArray();

        var feed = all.Where(i => i.ImageUri.ToString().Contains(Environment.CurrentImageOnScreen.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

        _realm.Write(() => feed.IsFavorite = false);

        await Application.Current.MainPage.DisplayAlert("Done", $"Content removed from favorites", "Ok");
    }
}