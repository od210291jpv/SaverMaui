using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        public static SearchResultsViewModel Instance;

        private ObservableCollection<KeyValuePair<string, SearchResult[]>> contentCollection;

        public ObservableCollection<KeyValuePair<string, SearchResult[]>> ContentCollection
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
                this.ContentCollection = new ObservableCollection<KeyValuePair<string, SearchResult[]>>();
            }
            catch { }
        }

        private KeyValuePair<string, SearchResult[]> currentCategory;


        public KeyValuePair<string, SearchResult[]> CurrentCategory
        {
            get => currentCategory;
            set => currentCategory = value;
        }

        public ICommand ItemChangedCommand { get; }

        public SearchResultsViewModel()
        {
            this.ContentCollection = new ObservableCollection<KeyValuePair<string, SearchResult[]>>();
            this.contentCollection = new ObservableCollection<KeyValuePair<string, SearchResult[]>>();

            this.currentCategory = new();
            this.CurrentCategory = new();
            this.ItemChangedCommand = new SearchCurrentItemChangedCommand(this);
            Instance = this;
        }
    }
}
