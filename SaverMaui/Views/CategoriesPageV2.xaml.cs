using CommunityToolkit.Maui.Core.Extensions;
using SaverMaui.Commands;
using SaverMaui.Services;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Views;

public partial class CategoriesPageV2 : ContentPage
{
	public CategoriesPageV2()
	{
		InitializeComponent();
        this.Appearing += OnAppearing;
    }

    private async void OnAppearing(object sender, EventArgs e)
    {
        var allCAts = await BackendServiceClient.GetInstance().CategoriesActions.GetAllCategories();
        if (CategoriesViewModel.Instance is not null) 
        {
            CategoriesViewModel.Instance.Categories.Clear();
            CategoriesViewModel.Instance.Categories = allCAts.OrderBy(c => c.Name).ToObservableCollection();
        }
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

    public async void OnCategoryOpen(object sender, EventArgs e) 
	{
        if (Environment.Login != null)
        {
            this.NavigateToFeedItemCommand.Execute(CategoriesViewModel.Instance);
        }
        else 
        {
            await Application.Current.MainPage.DisplayAlert("Unauthorized", $"Please login or create account to use categories functionality!", "Ok");
        }
    }

    public void OnAddToFavorites(object sender, EventArgs e) 
    {
        this.AddFavoriteCategoryCmd.Execute(CategoriesViewModel.Instance);
    }
}