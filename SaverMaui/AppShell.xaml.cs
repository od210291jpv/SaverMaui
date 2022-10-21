namespace SaverMaui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		if (DeviceInfo.Current.Platform.ToString().ToLower() != "android") 
		{
			AndroidCategories.IsVisible = false;
        }
		if (DeviceInfo.Current.Platform.ToString() != "WinUI") 
		{
			WindowsCategories.IsVisible = false;
		}
	}
}
