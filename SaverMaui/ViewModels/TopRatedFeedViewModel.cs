using SaverMaui.Custom_Elements;
using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class TopRatedFeedViewModel : BaseViewModel
    {
        public static TopRatedFeedViewModel Instance { get; private set; }


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

        private ImageRepresentationElement currentContent;

        public ImageRepresentationElement CurrentContent
        {
            get => currentContent;
            set 
            {
                this.currentContent = value;
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        public TopRatedFeedViewModel()
        {
            this.contentCollection = new ObservableCollection<ImageRepresentationElement>();
            this.ContentCollection = new ObservableCollection<ImageRepresentationElement>();

            Instance = this;
        }
    }
}
