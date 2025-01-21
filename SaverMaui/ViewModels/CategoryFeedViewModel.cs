using SaverMaui.Custom_Elements;
using SaverMaui.Services.Contracts.Content;
using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class CategoryFeedViewModel : BaseViewModel
    {
        public static CategoryFeedViewModel Instance { get; private set; }

        private ObservableCollection<ContentDto> contentCollection;

        public ObservableCollection<ContentDto> ContentCollection 
        { 
            get => this.contentCollection;
            set
            {
                this.contentCollection = value;
                OnPropertyChanged(nameof(ContentCollection));
            }
        }

        private ImageRepresentationElement currentImage;

        public ImageRepresentationElement CurrentImage { get => currentImage; set => currentImage = value; }

        public CategoryFeedViewModel()
        {
            this.ContentCollection = new ObservableCollection<ContentDto>();
            this.contentCollection = new ObservableCollection<ContentDto>();
            Instance = this;
        }
    }
}
