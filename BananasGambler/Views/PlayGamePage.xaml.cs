namespace BananasGambler.Views;

public partial class PlayGamePage : ContentPage
{
	public PlayGamePage()
	{
		InitializeComponent();
        this.PlayButton.Clicked += PlayButton_Clicked;
	}

    private async void PlayButton_Clicked(object sender, EventArgs e)
    {
        await this.PlayButton.RotateTo(360, 250);
        this.PlayButton.Rotation = 0;
    }
}