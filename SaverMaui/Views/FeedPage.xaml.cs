using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class FeedPage : ContentPage
{
	public FeedPage()
	{
		InitializeComponent();
	}

	private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
	{
        Realm _realm = Realm.GetInstance();
		var all = _realm.All<Content>().ToArray();

		var feed = all.Where(i => i.ImageUri.ToString().Contains(Environment.CurrentImageOnScreen.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

        _realm.Write(() => feed.IsFavorite = true);

        await Application.Current.MainPage.DisplayAlert("Done", $"Content added to favorites", "Ok");
    }
}