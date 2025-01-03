
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Custom_Elements;
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

    private static short CurrentPage = 0;
    private static bool InitialLoad = true;

    private async void OnPageAppearing(object sender, EventArgs e)
    {
        if (Environment.Password != null && Environment.Password != string.Empty && Environment.Login != null) 
        {
            Services.Contracts.Content.ContentDto[] allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentWithPaginationAsync(CurrentPage, 100);

            var ordered = allContent.OrderBy(i => i.DateCreated).ToArray();

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

                InitialLoad = false;
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

    private async void OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        if (InitialLoad == true) 
        {
            return;
        }

        var currentItem = (ImageRepresentationElement)e.CurrentItem;
        if (currentItem.Id == ContentCleanupViewModel.Instance?.ContentCollection.Last().Id) 
        {
            CurrentPage += 1;
            Services.Contracts.Content.ContentDto[] allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentWithPaginationAsync(CurrentPage, 100);

            var ordered = allContent.OrderBy(i => i.DateCreated).ToArray();

            if (allContent != null)
            {
                foreach (var item in ordered)
                {
                    ContentCleanupViewModel.Instance?.ContentCollection.Add(new ImageRepresentationElement()
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
}