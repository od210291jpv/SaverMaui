using DynamicData.Binding;
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

        public FeedViewModel()
        {
            this.contentCollection = new ObservableCollection<ImageRepresentationElement>();
            this.ContentCollection = new ObservableCollection<ImageRepresentationElement>();

            this.ItemChangedCommand = new FeedItemChangedCommand(this);

            Realm _realm = Realm.GetInstance();
            Content[] allRelatedContent = _realm.All<Content>().ToArray();

            ObservableCollection<ImageRepresentationElement> allFeed = new();

            foreach (var cat in allRelatedContent.ToArray().Reverse())
            {
                allFeed.Add(new ImageRepresentationElement()
                {
                    CategoryId = cat.CategoryId.Value,
                    Name = cat.Title,
                    Source = cat.ImageUri,
                    IsFavorite = cat.IsFavorite
                });
            }

            this.ContentCollection = allFeed;

            Instance = this;
        }
    }
}
