using Realms;
using SaverMaui.Commands;
using SaverMaui.Models;
using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class VideoManagementViewModel : BaseViewModel
    {
        private static VideoManagementViewModel instance;

        public static VideoManagementViewModel GetInstance() 
        {
            if (instance != null) 
            {
                return instance;
            }

            instance = new VideoManagementViewModel();
            return instance;
        }

        public ObservableCollection<Video> Videos { get; set; }

        private Video selectedVideo;

        public Video SelectedVideo 
        { 
            get => this.selectedVideo;
            set 
            {
                this.selectedVideo = value;
                OnPropertyChanged(nameof(SelectedVideo));
            }
        }

        private string addVideoUrl;

        public string AddVideoUrl 
        {
            get => this.addVideoUrl;
            set 
            {
                this.addVideoUrl = value;
                OnPropertyChanged(nameof(AddVideoUrl));
            }
        }

        private string addVideoName;

        public string AddVideoName 
        { 
            get => addVideoName;
            set 
            {
                this.addVideoName = value;
                OnPropertyChanged(nameof(AddVideoName));
            }
        }


        private AddVideoCommand addVideoCommand;

        public AddVideoCommand AddVideoCommand 
        { 
            get 
            {
                return this.addVideoCommand ?? (this.addVideoCommand = new AddVideoCommand(this));
            }
        }

        private SyncVideoCommand syncVideoCommand;

        public SyncVideoCommand SyncVideoCommand 
        {
            get 
            {
                return this.syncVideoCommand ?? (this.syncVideoCommand = new SyncVideoCommand());
            }
        }

        public VideoManagementViewModel()
        {
            instance = this;

            this.Videos = new ObservableCollection<Video>();

            Realm _realm = Realm.GetInstance();

            var allVideos = _realm.All<Video>();

            foreach ( Video video in allVideos.ToArray() ) 
            {
                this.Videos.Add( video );
            }
        }
    }
}
