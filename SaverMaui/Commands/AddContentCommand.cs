using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Newtonsoft.Json;
using Realms;
using RestSharp;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.Services.Helpers;
using SaverMaui.ViewModels;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Web;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class AddContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> execute;
        private SettingsViewModel viewModel;

        private bool allowAdd = false;

        public AddContentCommand(SettingsViewModel vm, Action<object> _execute)
        {
            this.execute = _execute;
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            Realm _realm = Realm.GetInstance();

            if (_realm != null)
            {
                return true;
            }
            return false;
        }

        public async void Execute(object parameter)
        {
            if (allowAdd == false) 
            {
                await Application.Current.MainPage.DisplayAlert("Forbidden", $"Content adding is blocked now by administration", "Ok");
                return;
            }

            if (viewModel.SelectedCategory is null)
            {
                Ping pingSender = new Ping();
                PingReply reply = await pingSender.SendPingAsync(UriHelper.ImageRecognitionHost, 1000);

                if (reply.Status == IPStatus.Success)
                {
                    RestClient client = new RestClient();
                    var ur = $"{UriHelper.ImageRecognitionApi}{HttpUtility.UrlEncode(this.viewModel.ContentUri)}";
                    var request = new RestRequest(ur, Method.Get);
                    var resp = await client.ExecuteGetAsync<string>(request);

                    bool answer = await Application.Current.MainPage.DisplayAlert("Suggestion",
                        $"The most suitable category for the content to be saved is:{resp.Content}. Would you like to use this category?", "Yes", "No");
                    if (answer == true) 
                    {
                        Realm _realm = Realm.GetInstance();

                        var cats = _realm.All<Category>().ToArray();
                        var expectedCat = JsonConvert.DeserializeObject<string>(resp.Content);

                        var reqCat = cats.FirstOrDefault(c => c.Name == expectedCat);

                        if (reqCat != null)
                        {
                            Content content = new Content()
                            {
                                CategoryId = reqCat.CategoryId,
                                ImageUri = this.viewModel.ContentUri,
                                Title = this.viewModel.ContentTitle
                            };

                            _realm.Write(() => _realm.Add<Content>(content));
                        }
                        else 
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", $"Required category {resp.Content} does not exist!", "Ok");
                        }
                    }

                }
                return;
            }

            this.execute(parameter);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"New content added!", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
            this.viewModel.ContentUri = "";

            SettingsViewModel.GetInstance().ContentAmount += 1;


            if (FeedViewModel.Instance != null) 
            {
                FeedViewModel.Instance.ContentCollection.Clear();

                Realm _realm = Realm.GetInstance();
                Content[] allRelatedContent = _realm.All<Content>().ToArray();

                ObservableCollection<ImageRepresentationElement> allFeed = new();

                foreach (var cat in allRelatedContent.ToArray().Reverse())
                {
                    allFeed.Add(new ImageRepresentationElement()
                    {
                        ContentId = cat.Id,
                        CategoryId = cat.CategoryId.Value,
                        Name = cat.Title,
                        Source = cat.ImageUri,
                        IsFavorite = cat.IsFavorite
                    });
                }

                FeedViewModel.Instance.ContentCollection = allFeed;
            }
        }
    }

}
