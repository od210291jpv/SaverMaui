using Realms;
using SaverMaui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaverMaui.ViewModels
{
    public class CategoryFeedViewModel : BaseViewModel
    {
        private ObservableCollection<Content> contentCollection;
        public ObservableCollection<Content> ContentCollection { get => contentCollection; set => contentCollection = value; }

        public CategoryFeedViewModel()
        {
            this.ContentCollection = new ObservableCollection<Content>();

            Realm _realm = Realm.GetInstance();
            Content[] allRelatedContent = _realm.All<Content>().ToArray();

            foreach (var cat in allRelatedContent.Where(c => c.CategoryId.Value == Environment.SahredData.currentCategory.CategoryId).ToArray())
            {
                ContentCollection.Add(cat);
            }
        }
    }
}
