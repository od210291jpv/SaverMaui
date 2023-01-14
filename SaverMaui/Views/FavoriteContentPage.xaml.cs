using Realms;
using SaverMaui.Models;

namespace SaverMaui.Views;

public partial class FavoriteContentPage : ContentPage
{
	public FavoriteContentPage()
	{
		InitializeComponent();
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