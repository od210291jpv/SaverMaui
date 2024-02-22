namespace SaverMaui.Views;
using Microsoft.Maui.Controls;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint;
using System;
using Plugin.CurrentActivity;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		this.Loaded += OnPageLoaded;
	}

    public async void OnPageLoaded(object sender, EventArgs e)
    {
        CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);

        var request = new AuthenticationRequestConfiguration("Please process bimetric authentication", "In case of authentication fail, the app will be closed");
        var result = await CrossFingerprint.Current.AuthenticateAsync(request);

        if (!result.Authenticated)
        {
            Application.Current.Quit();
        }
    }
}