namespace SaverMaui.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();

		if (Environment.ProfileData != null && Environment.ProfileData?.PublishedCategories != null) 
		{
            this.PublishedCats.Text = $"Amount of published categories: {Environment.ProfileData.PublishedCategories.Count()}";
        }
    }
}