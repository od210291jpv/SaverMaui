using Realms;
using SaverMaui.Commands;
using SaverMaui.Models;
using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class WebContentViewModel : BaseViewModel
    {
        public static WebContentViewModel Instance;

        public ObservableCollection<WebContent> AllWebContent { get; set; }


        private string contentUrl;

        public string ContentUrl 
        {
            get => this.contentUrl;
            set 
            {
                this.contentUrl = value;
                OnPropertyChanged(nameof(ContentUrl));
            }
        }

        private string contentTitle;

        public string ContentTitle 
        {
            get => this.contentTitle;
            set 
            {
                this.contentTitle = value;
                OnPropertyChanged("ContentTitle");
            }
        }

        private WebContent selectedWebContent;

        public WebContent SelectedWebContent 
        { 
            get => this.selectedWebContent;
            set 
            {
                this.selectedWebContent = value;
                WebContentViewModel.WebViewSource = selectedWebContent.Source;
                OnPropertyChanged("SelectedWebContent");
            }
        }

        public static string WebViewSource { get; set; }

        private AddWebContentCommand addWebContentCmd;

        public AddWebContentCommand AddWebContentCmd 
        {
            get 
            {
                return this.addWebContentCmd ??= new AddWebContentCommand(this);
            }
        }

        public void RefreshContentOnPage()
        {
            this.AllWebContent.Clear();


            Realm _realm = Realm.GetInstance();

            var allWebContent = _realm.All<WebContent>();

            foreach (var cnt in allWebContent) 
            {
                this.AllWebContent.Add(cnt);
            }
        }

        public WebContentViewModel()
        {
            this.AllWebContent = new ObservableCollection<WebContent>();
            this.RefreshContentOnPage();

            WebContentViewModel.Instance = this;
        }
    }
}
