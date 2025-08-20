
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SaverMaui.Custom_Elements;
using SaverMaui.Services;
using SaverMaui.ViewModels;

namespace SaverMaui.Views;

public partial class TiledFeed : ContentPage
{
    private static short CurrentPage = 0;
    private static bool InitialLoad = true;

    public TiledFeed()
	{
		InitializeComponent();
        this.Appearing += OnFeedAppearing;
    }

    private async void OnFeedAppearing(object sender, EventArgs e)
    {
        if (Environment.Login == null || Environment.Password == null)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"Only authorized users can access Live Feed", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);

            return;
        }

        var allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentWithPaginationAsync(CurrentPage, 50);

        if (allContent != null)
        {

            if (BindingContext is TiledFeedViewModel viewModel) 
            {
                viewModel.ContentCollection.Clear();

                foreach (var item in allContent.OrderByDescending(c => c.DateCreated))
                {
                    viewModel.ContentCollection.Add(new ImageRepresentationElement()
                    {
                        ContentId = item.Id,
                        Name = item.Title,
                        Source = new UriImageSource()
                        {
                            Uri = new Uri(item.ImageUri),
                            CachingEnabled = true,
                            CacheValidity = TimeSpan.FromDays(30)
                        },
                        CategoryId = item.CategoryId ?? new Guid()
                    });
                }
            }

            InitialLoad = false;
        }
    }

    private async void OnCurrentItemChanged(object sender, SelectionChangedEventArgs e)
    {
        if (InitialLoad == true)
        {
            return;
        }

        if (BindingContext is TiledFeedViewModel viewModel)
        {
            var currentItem = viewModel.CurrentImage;
            if (currentItem.Id == viewModel.ContentCollection.Last().Id)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make($"Loading more items...", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);

                CurrentPage += 1;
                Services.Contracts.Content.ContentDto[] allContent = await BackendServiceClient.GetInstance().ContentActions.GetAllContentWithPaginationAsync(CurrentPage, 50);

                if (allContent != null)
                {
                    foreach (var item in allContent.OrderBy(c => c.DateCreated))
                    {
                        viewModel.ContentCollection.Add(new ImageRepresentationElement()
                        {
                            ContentId = item.Id,
                            Name = item.Title,
                            Source = new UriImageSource()
                            {
                                Uri = new Uri(item.ImageUri),
                                CachingEnabled = true,
                                CacheValidity = TimeSpan.FromDays(30)
                            },
                            CategoryId = item.CategoryId ?? new Guid()
                        });
                    }
                }
            }
        }
    }
}