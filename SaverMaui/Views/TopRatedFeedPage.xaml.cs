using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

using Realms;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.ViewModels;
using System.Collections.ObjectModel;

namespace SaverMaui.Views
{
    public partial class TopRatedFeedPage : ContentPage
    {
        public TopRatedFeedPage()
        {
            InitializeComponent();
            this.Appearing += OnAppearing;
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            if (Environment.ProfileIntId != 0)
            {
                var allContent = await BackendServiceClient.GetInstance().ContentActions.GetRatedContent();
                ObservableCollection<ImageRepresentationElement> allFeed = new();

                var sortedContent = allContent.OrderByDescending(x => x.Rating);

                foreach (var cat in sortedContent)
                {
                    allFeed.Add(new ImageRepresentationElement()
                    {
                        CategoryId = cat.CategoryId.Value,
                        Name = cat.Title,
                        Source = cat.ImageUri,
                        IsFavorite = false,
                        ContentId = cat.Id,
                    });
                }

                TopRatedFeedViewModel.Instance.ContentCollection = allFeed;
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                var toast = Toast.Make($"You are authorized, online results are shown", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
            }
            else 
            {
                Realm _realm = Realm.GetInstance();
                Content[] allRelatedContent = _realm.All<Content>().Where(c => c.Rating > 0).OrderByDescending(c => c.Rating).ToArray();

                var zeroIdContent = _realm.All<Content>().Where(c => c.Id == 0 && c.ImageUri != string.Empty).ToArray();

                ObservableCollection<ImageRepresentationElement> allFeed = new();

                foreach (var cat in allRelatedContent.ToArray().Reverse())
                {
                    allFeed.Add(new ImageRepresentationElement()
                    {
                        CategoryId = cat.CategoryId.Value,
                        Name = cat.Title,
                        Source = cat.ImageUri,
                        IsFavorite = cat.IsFavorite,
                        ContentId = cat.Id,
                    });
                }

                TopRatedFeedViewModel.Instance.ContentCollection = allFeed;

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                var toast = Toast.Make($"You are not authorized, local results are shown", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var result = await BackendServiceClient.GetInstance().ContentActions.DeleteContentAsync(TopRatedFeedViewModel.Instance.CurrentContent.ContentId);

            Realm _realm = Realm.GetInstance();

            var img = _realm.All<Content>().ToArray().Where(i => i.ImageUri.ToString().Contains(TopRatedFeedViewModel.Instance.CurrentContent.Source.ToString().Replace("Uri: ", ""))).FirstOrDefault();

            if (img != null)
            {
                _realm.Write(() => _realm.Remove(img));
            }

            TopRatedFeedViewModel.Instance.ContentCollection.Remove(TopRatedFeedViewModel.Instance.CurrentContent);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var toast = Toast.Make($"Content was removed", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}