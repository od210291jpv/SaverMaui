using Realms;
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

        public FeedRandomContentViewModel()
        {            
            Instance = this;
        }
    }
}
