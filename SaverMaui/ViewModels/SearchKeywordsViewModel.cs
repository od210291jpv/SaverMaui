using SaverMaui.Services.Contracts.Search;
using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class SearchKeywordsViewModel : BaseViewModel
    {
        public static SearchKeywordsViewModel Instance { get; private set; }

        private ObservableCollection<KeywordResult> searchResults;
        private KeywordResult selectedKeyword;

        public void ClearContent()
        {
            try
            {
                //this.contentCollection.Clear();
                this.SearchResults = new ();
            }
            catch { }
        }

        public ObservableCollection<KeywordResult> SearchResults { get => searchResults; set => searchResults = value; }

        public KeywordResult SelectedKeyword 
        { 
            get => selectedKeyword;
            set 
            {
                selectedKeyword = value;
                OnPropertyChanged(nameof(SelectedKeyword));
            } 
        }

        public SearchKeywordsViewModel()
        {
            this.searchResults = new ObservableCollection<KeywordResult>();
            Instance = this;
        }
    }
}
