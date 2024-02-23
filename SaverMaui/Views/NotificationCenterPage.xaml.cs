using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class NotificationCenterPage : ContentPage
{
	public NotificationCenterPage()
	{
		InitializeComponent();
		//Loaded += OnLoadedAsync;
	}

    private async void OnLoadedAsync(object sender, EventArgs e)
    {
		//await NotificationCenterViewModel.GetInstance()?.Connect();
    }
}