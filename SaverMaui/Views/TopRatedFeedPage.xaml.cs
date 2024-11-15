using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.ViewModels;

namespace SaverMaui.Views
{
    public partial class TopRatedFeedPage : ContentPage
    {
        public TopRatedFeedPage()
        {
            InitializeComponent();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {

            await BackendServiceClient.GetInstance().ContentActions.DeleteContentAsync(TopRatedFeedViewModel.Instance.CurrentContent.ContentId);

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