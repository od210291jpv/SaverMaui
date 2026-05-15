namespace SaverMaui.Views;
using Microsoft.Maui.Controls;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
        try
        {
            InitializeComponent();
            this.Loaded += OnPageLoaded;
        }
        catch (Exception ex)
        {
            // Поставте тут точку зупинки (breakpoint) під час дебагу
            // і перевірте значення ex.InnerException.Message
            var realError = ex.InnerException?.Message ?? ex.Message;
            System.Diagnostics.Debug.WriteLine($"КРИТИЧНА ПОМИЛКА XAML: {realError}");
            throw;
        }

	}

    public async void OnPageLoaded(object sender, EventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.Android) 
        {
            var request = new AuthenticationRequestConfiguration("Please process bimetric authentication", "In case of authentication fail, the app will be closed");
            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            if (!result.Authenticated)
            {
                Application.Current.Quit();
            }
        }
    }
}