using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class CategoryFeedPage : ContentPage
{
    private Category currentCat;

    public Category CurrentCat { get => currentCat; set => currentCat = value; }


    public CategoryFeedPage()
    {
        InitializeComponent();

        this.Title = Environment.SahredData.currentCategory.Name;
    }

    private void OnTapGestureRecognizerTapped(object sender, EventArgs e)
    {

    }
}