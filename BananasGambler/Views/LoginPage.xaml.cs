namespace BananasGambler.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        this.LoginButton.Clicked += LoginButtonClicked;
	}

    private async void LoginButtonClicked(object sender, EventArgs e)
    {
        LoginButton.Opacity = 0;
        await LoginButton.FadeTo(1, 4000);
    }
}