using Realms;
using SaverMaui.Models;
using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class VideoViewModel : BaseViewModel
    {
        private ObservableCollection<Video> videos;

        public ObservableCollection<Video> Videos { get { return videos; } set => videos = value; }

        public string VideoToPlay { get; set; }

        public VideoViewModel()
        {

            this.Videos = new ObservableCollection<Video>();

            Realm _realm = Realm.GetInstance();

            var allVideos = _realm.All<Video>();

            foreach (var video in allVideos) 
            {
                this.Videos.Add(video);
            }
        }
    }
}
