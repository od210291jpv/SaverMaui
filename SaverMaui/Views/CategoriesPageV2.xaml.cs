using SaverMaui.Commands;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Views;

public partial class CategoriesPageV2 : ContentPage
{
	public CategoriesPageV2()
	{
		InitializeComponent();
    }

    private NavigateToPageCommand navigateToFeedItemCommand;

    public NavigateToPageCommand NavigateToFeedItemCommand
    {
        get
        {
            return navigateToFeedItemCommand ??
                (navigateToFeedItemCommand = new NavigateToPageCommand(CategoriesViewModel.Instance));
        }
    }

    private AddFavoriteCategoryCommand addFavoriteCategoryCmd;

    public AddFavoriteCategoryCommand AddFavoriteCategoryCmd
    {
        get 
        {
            return addFavoriteCategoryCmd ?? 
                (addFavoriteCategoryCmd = new AddFavoriteCategoryCommand(CategoriesViewModel.Instance));
        }
    }

    public void OnCategoryOpen(object sender, EventArgs e) 
	{
        this.NavigateToFeedItemCommand.Execute(CategoriesViewModel.Instance);
    }

    public void OnAddToFavorites(object sender, EventArgs e) 
    {
        this.AddFavoriteCategoryCmd.Execute(CategoriesViewModel.Instance);
    }
}