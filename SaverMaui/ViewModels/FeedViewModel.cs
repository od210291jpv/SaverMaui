using Realms;

using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;

using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class FeedViewModel : BaseViewModel
    {
        public static FeedViewModel Instance { get; private set; }

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

        public ICommand ItemChangedCommand 
        {
            get;
        }

        public ICommand RateImageCommand 
        { 
            get;
        }

        public FeedViewModel()
        {
            this.contentCollection = new ObservableCollection<ImageRepresentationElement>();
            this.ContentCollection = new ObservableCollection<ImageRepresentationElement>();

            this.ItemChangedCommand = new FeedItemChangedCommand(this);
            this.RateImageCommand = new RateContentCommand(this);

            //Content[] allRelatedContent = Realm.GetInstance().All<Content>().ToArray();

            //foreach (var cont in allRelatedContent.Reverse())
            //{
            //    this.ContentCollection.Add(new ImageRepresentationElement()
            //    {
            //        CategoryId = cont.CategoryId.Value,
            //        Name = cont.Title,
            //        Source = cont.ImageUri,
            //        IsFavorite = cont.IsFavorite
            //    });
            //}

            Instance = this;
        }
    }
}
