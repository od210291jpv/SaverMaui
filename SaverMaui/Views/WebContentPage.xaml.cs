using SaverMaui.Commands;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class WebContentPage : ContentPage
{
	private NavigateToWebContentCommand navgateToWebContentCommand;

	public NavigateToWebContentCommand NavigateToWebContentCommand 
	{
		get => this.navgateToWebContentCommand ?? (this.navgateToWebContentCommand = new NavigateToWebContentCommand(WebContentViewModel.Instance));
	}

	public WebContentPage()
	{
		InitializeComponent();
	}

	public async void OnWebContentOpen(object sender, EventArgs e) 
	{
        NavigateToWebContentCommand.Execute(WebContentViewModel.Instance);

    }
}