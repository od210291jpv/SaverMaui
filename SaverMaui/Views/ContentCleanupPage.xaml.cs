
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class ContentCleanupPage : ContentPage
{
	public ContentCleanupPage()
	{
		InitializeComponent();
		this.Appearing += OnPageAppearing;
	}

    private async void OnPageAppearing(object sender, EventArgs e)
    {
        if (Environment.Password != null && Environment.Password != string.Empty && Environment.Login != null) 
        {
            Services.Contracts.Content.ContentDto[] allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentAsync();
            var ordered = allContent.OrderBy(i => i.DateCreated).ToArray();
            var l = ordered.Last();
            var f = ordered.First();

            if (allContent != null)
            {
                ContentCleanupViewModel.Instance.ContentCollection.Clear();

                foreach (var item in ordered)
                {
                    ContentCleanupViewModel.Instance?.ContentCollection.Add(new Custom_Elements.ImageRepresentationElement()
                    {
                        ContentId = item.Id,
                        Name = item.Title,
                        Source = item.ImageUri,
                        CategoryId = item.CategoryId ?? new Guid()
                    });
                }
            }
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {

        await BackendServiceClient.GetInstance().ContentActions.DeleteContentAsync(ContentCleanupViewModel.Instance.CurrentContent.ContentId);

        Realm _realm = Realm.GetInstance();

        var img = _realm.All<Content>().ToArray().Where(i => i.ImageUri.ToString().Contains(ContentCleanupViewModel.Instance.CurrentContent.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

        if (img != null)
        {
            _realm.Write(() => _realm.Remove(img));
        }

        ContentCleanupViewModel.Instance.ContentCollection.Remove(ContentCleanupViewModel.Instance.CurrentContent);

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var toast = Toast.Make($"Content was removed", ToastDuration.Short, 14);
        await toast.Show(cancellationTokenSource.Token);
        
    }
}