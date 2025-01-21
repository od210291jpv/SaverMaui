using CommunityToolkit.Maui.Core.Extensions;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class CategoryFeedPage : ContentPage
{
    private Category currentCat;

    public Category CurrentCat { get => currentCat; set => currentCat = value; }

    public Button ActionsButton;

    public CategoryFeedPage()
    {
        InitializeComponent();
        this.Appearing += OnAppearing;
        this.Title = CategoriesViewModel.Instance.SelectedCategory.Name;
    }

    private async void OnAppearing(object sender, EventArgs e)
    {
        Services.Contracts.Content.ContentDto[] relatedContent = await BackendServiceClient.GetInstance().CategoriesActions.GetCategoryContent(CategoriesViewModel.Instance.SelectedCategory.CategoryId);
        CategoryFeedViewModel.Instance?.ContentCollection.Clear();

        foreach (var relatedContentDto in relatedContent) 
        {
            CategoryFeedViewModel.Instance?.ContentCollection.Add(relatedContentDto);
        }
    }

    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
    }
}