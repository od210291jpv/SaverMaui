namespace SaverMaui.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        this.Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {        
    }

    private void Handle_Toggled(object sender, ToggledEventArgs e)
    {
        Environment.SearchResultsResfresh = e.Value;
    }
}