using SaverMaui.Commands;
using SaverMaui.Custom_Elements;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        public static SearchResultsViewModel Instance;

        private ObservableCollection<KeyValuePair<string, SearchResult[]>> contentCollection;
        private int currentKeyword;

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
                this.contentCollection.Clear();
                //this.ContentCollection = new ObservableCollection<KeyValuePair<string, SearchResult[]>>();
            }
            catch { }
        }

        //private KeyValuePair<string, SearchResult[]> currentKeyword;

        //public KeyValuePair<string, SearchResult[]> CurrentKeyword
        //{
        //    get => currentKeyword;
        //    set 
        //    {
        //        currentKeyword = value;
        //        OnPropertyChanged(nameof(CurrentKeyword));
        //    }
        //}

        public int CurrentKeyword 
        { 
            get => currentKeyword;
            set 
            {
                currentKeyword = value;
                OnPropertyChanged(nameof(CurrentKeyword));
            }
        }

        public SearchResultsViewModel()
        {
            this.contentCollection = new ObservableCollection<KeyValuePair<string, SearchResult[]>>();

            //this.currentKeyword = new();
            //this.CurrentKeyword = new();

            Instance = this;
        }
    }
}
