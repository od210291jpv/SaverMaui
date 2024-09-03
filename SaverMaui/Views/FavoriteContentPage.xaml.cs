using Realms;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class FavoriteContentPage : ContentPage
{
	public FavoriteContentPage()
	{
		InitializeComponent();
        this.Appearing += OnAppearing;
	}

    private void OnAppearing(object sender, EventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        Content[] allRelatedContent = _realm.All<Content>().Where(ct => ct.IsFavorite == true).ToArray();

        if (FavoriteContentViewModel.Instance != null) 
        {
            foreach (var cat in allRelatedContent)
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