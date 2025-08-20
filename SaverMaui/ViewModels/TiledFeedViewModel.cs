using SaverMaui.Custom_Elements;
using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class TiledFeedViewModel : BaseViewModel
    {
        private ObservableCollection<ImageRepresentationElement> contentCollection;

        public ObservableCollection<ImageRepresentationElement> ContentCollection
        {
            get => this.contentCollection;
            set
            {
                this.contentCollection = value;
                OnPropertyChanged(nameof(ContentCollection));
            }
        }

        private ImageRepresentationElement currentImage;

        public ImageRepresentationElement CurrentImage 
        { 
            get => currentImage;
            set 
            {
                this.currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        public TiledFeedViewModel()
        {
            this.ContentCollection = new ObservableCollection<ImageRepresentationElement>();
        }
    }
}
