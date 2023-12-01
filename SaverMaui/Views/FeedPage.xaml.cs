using Realms;
using SaverMaui.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

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
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make($"Content added to favorites", ToastDuration.Short, 14);
        await toast.Show(cancellationTokenSource.Token);

        //await Application.Current.MainPage.DisplayAlert("Done", $"Content added to favorites", "Ok");
    }

	private async void OnPinchGestureTapped(object sender, EventArgs e) 
	{
        Realm _realm = Realm.GetInstance();
        var all = _realm.All<Content>().ToArray();

        Environment.ImagesToDelete.Add(all.Where(c => c.ImageUri.ToString().Contains(Environment.CurrentImageOnScreen.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault().ImageUri);
        
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make($"Content added to delete list", ToastDuration.Short, 14);
        await toast.Show(cancellationTokenSource.Token);
    }
}