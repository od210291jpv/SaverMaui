using SaverMaui.Models;

namespace SaverMaui.Views;

public partial class CategoryFeedPage : ContentPage
{
    private Category currentCat;

    public Category CurrentCat { get => currentCat; set => currentCat = value; }

    public Button ActionsButton;

    public CategoryFeedPage()
    {
        InitializeComponent();
        this.Title = Environment.SahredData.currentCategory.Name;
    }

    private void OnTapGestureRecognizerTapped(object sender, EventArgs e)
    {
    }

    private void OnActionsClicked(object sender, EventArgs e)
    {
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
    }

    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
    }
}