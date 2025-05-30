using Realms;
using SaverMaui.Models;
using SaverMaui.SignalRModels;
using SaverMaui.ViewModels;
using System.Reactive.Linq;

namespace SaverMaui.Views;

public partial class ProfilePage : ContentPage
{
    private bool IsProgressSubscribed { get; set; }

	public ProfilePage()
	{
		InitializeComponent();

		if (Environment.ProfileData != null && Environment.ProfileData?.PublishedCategories != null) 
		{
            this.PublishedCats.Text = $"Amount of published categories: {Environment.ProfileData.PublishedCategories.Count()}";
        }
		this.Appearing += OnProfileAppearing;
    }

    private void OnProfileAppearing(object sender, EventArgs e)
    {
        Realm _realm = Realm.GetInstance();
        var allCats = _realm.All<Category>();

        this.TotalContentAmount.Text = $"{_realm.All<Content>().ToArray().Length}/{_realm.All<Content>().Where(c => c.Rating > 0).Count()}";

        var assCat = allCats.First(c => c.Name == "Ass");
        var totalAssContent = _realm.All<Content>().Where(c => c.CategoryId == assCat.CategoryId).ToArray();
        var ratedAssContent = totalAssContent.Where(c => c.Rating > 0).ToArray();

        this.TotalAssAmount.Text = $"{totalAssContent.Length}/{ratedAssContent.Length}";

        var bjCat = allCats.First(c => c.Name == "Bj");
        var totalBjContent = _realm.All<Content>().Where(c => c.CategoryId == bjCat.CategoryId).ToArray();
        var ratedBjContent = totalBjContent.Where(c => c.Rating > 0).ToArray();

        this.TotalBjAmount.Text = $"{totalBjContent.Length}/{ratedBjContent.Length}";

        var blondesCat = allCats.First(c => c.Name == "Blondes");
        var blondesContent = _realm.All<Content>().Where(c => c.CategoryId == blondesCat.CategoryId).ToArray();
        var ratedBlondesContent = blondesContent.Where(c => c.Rating > 0).ToArray();

        this.TotalBlondesAmount.Text = $"{blondesContent.Length}/{ratedBlondesContent.Length}";

        var brunettesCat = allCats.First(c => c.Name == "Brunettes");
        var brunettesContent = _realm.All<Content>().Where(c => c.CategoryId == brunettesCat.CategoryId).ToArray();
        var ratedBrunettes = brunettesContent.Where(c => c.Rating > 0).ToArray();

        this.TotalBrunettesAmount.Text = $"{brunettesContent.Length}/{ratedBrunettes.Length}";

        var cumfaceCat = allCats.First(c => c.Name == "Cumface");
        var cumfaceContent = _realm.All<Content>().Where(c => c.CategoryId == cumfaceCat.CategoryId).ToArray();
        var ratedCumfaceContent = cumfaceContent.Where(c => c.Rating > 0).ToArray();

        this.TotalCumfaceAmount.Text = $"{cumfaceContent.Length}/{ratedCumfaceContent.Length}";

        var curvysCat = allCats.First(c => c.Name == "Curvys");
        var curvysContent = _realm.All<Content>().Where(c => c.CategoryId == curvysCat.CategoryId).ToArray();
        var ratedCurvys = curvysContent.Where(c => c.Rating > 0);

        this.TotalCurvysAmount.Text = $"{curvysContent.Length}/{ratedCurvys.Count()}";

        var faceCat = allCats.First(c => c.Name == "Face");
        var faceContent = _realm.All<Content>().Where(c => c.CategoryId == faceCat.CategoryId).ToArray();
        var ratedFace = faceContent.Where(c => c.Rating > 0).ToArray();

        this.TotalFaceAmount.Text = $"{faceContent.Length}/{ratedFace.Length}";

        var lipsCat = allCats.First(c => c.Name == "Lips");
        var allLipsContent = _realm.All<Content>().Where(c => c.CategoryId == lipsCat.CategoryId).ToArray();
        var ratedLips = allLipsContent.Where(c => c.Rating > 0).ToArray();

        this.TotalLipsAmount.Text = $"{allLipsContent.Length}/{ratedLips.Length}";

        var milfCat = allCats.First(c => c.Name == "MILF");
        var milfContent = _realm.All<Content>().Where(c => c.CategoryId == milfCat.CategoryId).ToArray();
        var ratedMilf = milfContent.Where(c => c.Rating > 0).ToArray();

        this.TotalMilfAmount.Text = $"{milfContent.Length}/{ratedMilf.Length}";

        var redCat = allCats.First(c => c.Name == "Red");
        var allredContent = _realm.All<Content>().Where(c => c.CategoryId == redCat.CategoryId).ToArray();
        var ratedRed = allredContent.Where(c => c.Rating > 0).ToArray();

        this.TotalRedAmount.Text = $"{allredContent.Length}/{ratedRed.Length}";

        var restCat = allCats.First(c => c.Name == "Rest");
        var restContent = _realm.All<Content>().Where(c => c.CategoryId == restCat.CategoryId).ToArray();
        var ratedRest = restContent.Where(c => c.Rating > 0).ToArray();

        this.TotalRestAmount.Text = $"{restContent.Length}/{ratedRest.Length}";

        var slutsCat = allCats.First(c => c.Name == "Sluts");
        var slutsContent = _realm.All<Content>().Where(c => c.CategoryId == slutsCat.CategoryId).ToArray();
        var ratedSluts = slutsContent.Where(c => c.Rating > 0).ToArray();

        this.TotalSlutsAmount.Text = $"{slutsContent.Length}/{ratedSluts.Length}";

        var titsCat = allCats.First(c => c.Name == "Tits");
        var titsContent = _realm.All<Content>().Where(c => c.CategoryId == titsCat.CategoryId).ToArray();
        var ratedTits = titsContent.Where(c => c.Rating > 0).ToArray();

        this.TotalTitsAmount.Text = $"{titsContent.Length}/{ratedTits.Length}";

        if (this.IsProgressSubscribed == false) 
        {
            NotificationCenterViewModel.GetInstance()?.Notifications.Where(n => n.Message.Contains("Search Progress:")).Subscribe(OnProgressUpdatedFiltered);
            this.IsProgressSubscribed = true;
        }
    }

    public async void OnProgressUpdatedFiltered(Notification notification) 
    {
        var progressValue = notification.Message.Split(":").Last();
        double result;
        var parsed = double.TryParse(progressValue, System.Globalization.CultureInfo.InvariantCulture, out result);

        if (parsed == true) 
        {
            await this.SearchProgress.ProgressTo(result, 500, Easing.Linear);
        }
    }
}