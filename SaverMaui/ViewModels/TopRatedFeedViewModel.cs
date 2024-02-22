using Realms;
using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private ImageRepresentationElement currentImage;

        public ImageRepresentationElement CurrentContent
        {
            get => currentImage;
            set => currentImage = value;
        }



        public TopRatedFeedViewModel()
        {
            this.contentCollection = new ObservableCollection<ImageRepresentationElement>();
            this.ContentCollection = new ObservableCollection<ImageRepresentationElement>();


            Realm _realm = Realm.GetInstance();
            Content[] allRelatedContent = _realm.All<Content>().Where(c => c.Rating > 0).OrderByDescending(c => c.Rating).ToArray();

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
