using BananasGambler.Commands;
using BananasGambler.ViewModels;

namespace BananasGambler.Views;

public partial class PacksPage : ContentPage
{
	public PacksPage()
	{
		InitializeComponent();
	}

    private NavigateToPackCommand navigateToFeedItemCommand { get; set; }

    internal NavigateToPackCommand NavigateToFeedItemCommand
    {
        get
        {
            return navigateToFeedItemCommand ??
                (this.navigateToFeedItemCommand = new NavigateToPackCommand(PacksPageViewModel.Instance));
        }
    }

    public void OnCategoryOpen(object sender, EventArgs e)
    {
        this.NavigateToFeedItemCommand.Execute(PacksPageViewModel.Instance);
    }
}