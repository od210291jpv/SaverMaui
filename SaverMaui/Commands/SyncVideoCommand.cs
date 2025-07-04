using Realms;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts.Video;
using SaverMaui.Services.ServiceExtensions;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class SyncVideoCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            BackendServiceClient.GetInstance();
            
            Realm _realm = Realm.GetInstance();

            var allLocalVideo = _realm.All<Video>().ToArray();
            var allBackendVideo = await BackendServiceClient.GetInstance().GetVideoRequest(Environment.ProfileData.ProfileId);
            var backendVideoUris = allBackendVideo.Select(v => v.ImageUri).ToArray();

            if (allLocalVideo.Length > allBackendVideo.Length) 
            {
                var videoToBeAdded = allLocalVideo.Where(c => backendVideoUris.Contains(c.VideoUri) == false).ToArray();

                if (videoToBeAdded.Length > 0)
                {
                    SyncVideoRequestDto requestModel = new SyncVideoRequestDto();
                    requestModel.PublisherId = Environment.ProfileData.ProfileId;
                    requestModel.VideoContent = videoToBeAdded.Select(v => new AddVideoRequestDto()
                    {
                        CategoryId = v.CategoryId,
                        DateCreated = DateTime.Now,
                        ImageUri = v.VideoUri,
                        PublisherProfileId = Guid.Parse("2409395f-16b6-4a89-b2c4-cb70f376d7c3"),
                        Title = v.Title,
                    }).ToArray();

                    var result = await BackendServiceClient.GetInstance().AddVideoRequest(requestModel);

                    if (result != null && result.Length > 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ok", $"{result.Length} videos where synced with backend", "Ok");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Warning", $"Something went wrong!", "Ok");
                    }
                }
                else 
                {
                    await Application.Current.MainPage.DisplayAlert("Info", $"Video content is up to date!", "Ok");
                }
            }

            if (backendVideoUris.Length > allLocalVideo.Length) 
            {
                var videoToBePulled = allBackendVideo.Where(c => allLocalVideo.Select(s => s.VideoUri).ToArray().Contains(c.ImageUri) == false).ToArray();

                foreach (var v in videoToBePulled) 
                {
                    Video video = new Video()
                    {
                        CategoryId = Guid.NewGuid(),
                        IsFavorite = false,
                        Title = v.Title,
                        VideoUri = v.ImageUri
                    };

                    _realm.Write(() => _realm.Add(video));
                }

                await Application.Current.MainPage.DisplayAlert("Ok", $"{videoToBePulled.Length} where pulled from backend", "Ok");
            }
        }
    }
}
