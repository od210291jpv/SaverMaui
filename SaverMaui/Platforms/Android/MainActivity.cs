using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;

namespace SaverMaui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle bundle) 
    {
        base.OnCreate(bundle);
        CrossCurrentActivity.Current.Init(this, bundle);

    }
}
