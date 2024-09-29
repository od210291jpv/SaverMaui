using Realms;
using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;

namespace SaverMaui.ViewModels
{
    public class FeedRandomContentViewModel : BaseViewModel
    {
        public static FeedRandomContentViewModel Instance { get; private set; }

        private ImageRepresentationElement currentImage;

        public ImageRepresentationElement CurrentImage
        {
            get { return currentImage; }
            set
            {
                currentImage = value;
                OnPropertyChanged("CurrentImage");
            }
        }

        private bool isRefreshing {  get; set; }

        public bool IsRefreshing 
        {
            get => this.isRefreshing;
            set 
            {
                this.isRefreshing = value;
                OnPropertyChanged("IsRefreshing");
            }
        }

        public RandomContentRefreshCommand RefreshCommand { get; set; }

        public FeedRandomContentViewModel()
        {
            RefreshCommand = new(this);
            Instance = this;
        }
    }
}
