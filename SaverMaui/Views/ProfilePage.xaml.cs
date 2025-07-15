using Realms;
using SaverMaui.Models;
using SaverMaui.Services;
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

		this.Appearing += OnProfileAppearing;
    }

    private async void OnProfileAppearing(object sender, EventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        var allCats = _realm.All<Category>();

        this.TotalContentAmount.Text = $"{await BackendServiceClient.GetInstance().ContentActions.GetAllContentCount()}";
        var funds = await BackendServiceClient.GetInstance().UserActions.GetProfileInfo(Environment.Login, Environment.Password);

        this.Funds.Text = $"{funds.Funds}";

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