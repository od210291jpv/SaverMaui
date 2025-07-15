using Realms;
using SaverMaui.Models;
using SaverMaui.SignalRModels;
using SaverMaui.ViewModels;
using System.Reactive.Linq;

namespace SaverMaui.Views;

public partial class ProfilePage : ContentPage
{
    private bool IsProgressSubscribed { get; set; }

	public ProfilePage()
	{
		InitializeComponent();

		if (Environment.ProfileData != null && Environment.ProfileData?.PublishedCategories != null) 
		{
            this.PublishedCats.Text = $"Amount of published categories: {Environment.ProfileData.PublishedCategories.Count()}";
        }
		this.Appearing += OnProfileAppearing;
    }

    private void OnProfileAppearing(object sender, EventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        var allCats = _realm.All<Category>();

        this.TotalContentAmount.Text = $"{_realm.All<Content>().ToArray().Length}/{_realm.All<Content>().Where(c => c.Rating > 0).Count()}";

       

        if (this.IsProgressSubscribed == false) 
        {
            NotificationCenterViewModel.GetInstance()?.Notifications.Where(n => n.Message.Contains("Search Progress:")).Subscribe(OnProgressUpdatedFiltered);
            this.IsProgressSubscribed = true;
        }
    }

    public async void OnProgressUpdatedFiltered(Notification notification) 
    {
        var progressValue = notification.Message.Split(":").Last();
        double result;
        var parsed = double.TryParse(progressValue, System.Globalization.CultureInfo.InvariantCulture, out result);

        if (parsed == true) 
        {
            await this.SearchProgress.ProgressTo(result, 500, Easing.Linear);
        }
    }
}