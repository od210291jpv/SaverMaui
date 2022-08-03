using SaverMaui.Models;

namespace SaverMaui.Views;

public partial class CategoryFeedPage : ContentPage
{
    private Category currentCat;

    public Category CurrentCat { get => currentCat; set => currentCat = value; }


    public CategoryFeedPage(Category category)
    {
        InitializeComponent();

        this.Title = category.Name;
    }
}