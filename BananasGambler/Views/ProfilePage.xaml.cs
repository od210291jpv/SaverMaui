namespace BananasGambler.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
        this.BuyCardButton.Clicked += BuyCardButtonClicked;
	}

    private async void BuyCardButtonClicked(object sender, EventArgs e)
    {
        await this.BuyCardButton.ScaleTo(0.3, 1000);
        await this.BuyCardButton.ScaleTo(1, 1000);
    }
}