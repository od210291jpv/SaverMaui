using Realms;

using SaverMaui.Models;

using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class FeedViewModel : BaseViewModel
    {
        private ObservableCollection<Content> contentCollection;
        public ObservableCollection<Content> ContentCollection { get => contentCollection; set => contentCollection = value; }

        public FeedViewModel()
        {
            this.ContentCollection = new ObservableCollection<Content>();

            Realm _realm = Realm.GetInstance();
            Content[] allRelatedContent = _realm.All<Content>().ToArray();

            foreach (var cat in allRelatedContent.ToArray().Reverse())
            {
                ContentCollection.Add(cat);
            }
        }
    }
}
