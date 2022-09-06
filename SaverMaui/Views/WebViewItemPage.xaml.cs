using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class WebViewItemPage : ContentPage
{
    public WebViewItemPage()
	{
		this.Content = new WebView() { Source = Environment.SahredData.currentWebContent.Source};
		
		InitializeComponent();
	}
}