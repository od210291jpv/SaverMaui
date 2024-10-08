using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class SearchCategoryFeedViewModel : BaseViewModel
    {
        public static SearchCategoryFeedViewModel instance { get; private set; }

        private ObservableCollection<SearchResult> searchResults;

        public ObservableCollection<SearchResult> SearchResults 
        { 
            get => searchResults;
            set => searchResults = value; 
        }

        private SearchResult currentResult;

        public SearchResult CurrentResult { get => currentResult; set => currentResult = value; }


        public ICommand CurrentImageChanged { get; }

        public SearchCategoryFeedViewModel()
        {
            this.SearchResults = new ObservableCollection<SearchResult>();
            this.Title = Environment.CurrectSearchResultCategory.Key;

            this.CurrentImageChanged = new SearchResultCategoryImageChangedCommand(this);

            instance = this;
        }
    }
}
