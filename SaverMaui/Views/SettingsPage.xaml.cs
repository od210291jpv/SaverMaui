using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        this.Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object sender, EventArgs e)
    {
        var vm = SettingsViewModel.GetInstance();
        Realm _realm = Realm.GetInstance();

        var allCategories = _realm.All<Category>().OrderBy(c => c.Name).ToArray();

        foreach (var cat in allCategories)
        {
            await Task.Delay(50);
            vm.Categories.Add(cat);
        }
        
    }
}