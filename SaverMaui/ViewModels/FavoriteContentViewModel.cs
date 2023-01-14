using Realms;
using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class FavoriteContentViewModel : BaseViewModel
    {
        private ObservableCollection<ImageRepresentationElement> contentCollection;
        public ObservableCollection<ImageRepresentationElement> ContentCollection { get => contentCollection; set => contentCollection = value; }

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

        public FavoriteContentViewModel()
        {
            this.contentCollection = new ObservableCollection<ImageRepresentationElement>();

            this.ItemChangedCommand = new FavoriteContentFeedItemChangedCommand(this);

            Realm _realm = Realm.GetInstance();
            Content[] allRelatedContent = _realm.All<Content>().Where(ct => ct.IsFavorite == true).ToArray();

            foreach (var cat in allRelatedContent)
            {
                ContentCollection.Add(new ImageRepresentationElement()
                {
                    CategoryId = cat.CategoryId.Value,
                    Name = cat.Title,
                    Source = cat.ImageUri,
                    IsFavorite = cat.IsFavorite
                });
            }
        }
    }
}
