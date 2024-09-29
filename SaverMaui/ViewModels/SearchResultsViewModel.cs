using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        public static SearchResultsViewModel Instance;

        private ObservableCollection<SearchResult> contentCollection;

        public ObservableCollection<SearchResult> ContentCollection
        {
            get => this.contentCollection;
            set
            {
                this.contentCollection = value;
                OnPropertyChanged(nameof(ContentCollection));
            }
        }

        public void ClearContent() 
        {
            try 
            {
                this.ContentCollection = new ObservableCollection<SearchResult>();

            }
            catch { }
        }

        private SearchResult currentImage;


        public SearchResult CurrentContent
        {
            get => currentImage;
            set => currentImage = value;
        }

        public ICommand ItemChangedCommand { get; }

        public SearchResultsViewModel()
        {
            this.ContentCollection = new ObservableCollection<SearchResult>();
            this.contentCollection = new ObservableCollection<SearchResult>();
            this.ItemChangedCommand = new SearchCurrentItemChangedCommand(this);
            Instance = this;
        }
    }
}
