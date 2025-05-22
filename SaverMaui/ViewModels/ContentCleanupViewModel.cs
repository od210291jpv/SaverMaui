using SaverMaui.Custom_Elements;
using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class ContentCleanupViewModel : BaseViewModel
    {
        public static ContentCleanupViewModel Instance { get; private set; }

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

        public ImageRepresentationElement CurrentContent
        {
            get => currentImage;
            set => currentImage = value;
        }

        public ContentCleanupViewModel()
        {
            this.contentCollection = new ObservableCollection<ImageRepresentationElement>();
            this.ContentCollection = new ObservableCollection<ImageRepresentationElement>();

            Instance = this;
        }
    }
}
