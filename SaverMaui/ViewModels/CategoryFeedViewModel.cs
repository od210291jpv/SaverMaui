using Realms;
using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class CategoryFeedViewModel : BaseViewModel
    {
        private ObservableCollection<ImageRepresentationElement> contentCollection;
        public ObservableCollection<ImageRepresentationElement> ContentCollection { get => contentCollection; set => contentCollection = value; }

        private ImageRepresentationElement currentImage;

        public ImageRepresentationElement CurrentImage { get => currentImage; set => currentImage = value; }

        public ICommand LogImageLoadCommand 
        { 
            get;
        }

        public CategoryFeedViewModel()
        {
            this.ContentCollection = new ObservableCollection<ImageRepresentationElement>();
            this.LogImageLoadCommand = new CategoryFeedCarouselItemChangedCommand(this);

            Realm _realm = Realm.GetInstance();
            Content[] allRelatedContent = _realm.All<Content>().ToArray();

            foreach (var cat in allRelatedContent.Where(c => c.CategoryId.Value == Environment.SahredData.currentCategory.CategoryId).ToArray())
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
