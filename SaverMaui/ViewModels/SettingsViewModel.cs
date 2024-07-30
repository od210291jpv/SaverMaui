using CommunityToolkit.Maui.Core.Extensions;
using Realms;

using SaverMaui.Commands;
using SaverMaui.Models;

using System.Collections.ObjectModel;

namespace SaverMaui.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string hostIpAddress;

        private static SettingsViewModel instance;

        public static SettingsViewModel GetInstance() 
        {
            if(instance is null) 
            {
                instance = new SettingsViewModel();
            }

            return instance;
        }

        public string HostIpAddress
        {
            get { return hostIpAddress; }
            set
            {
                hostIpAddress = value;
                OnPropertyChanged("HostIpAddress");
            }
        }

        public ObservableCollection<Category> Categories 
        {
            get 
            {
                return Realm.GetInstance().All<Category>().OrderBy(c => c.Name).ToObservableCollection();
            }
            set 
            {
                OnPropertyChanged("Categories");
            }
        }

        private string newCategoryName;

        public string NewCategoryName
        {
            get => newCategoryName;
            set { this.newCategoryName = value; OnPropertyChanged("NewCategoryName"); }
        }

        private string feedUrl;

        public string FeedUrl 
        {
            get => this.feedUrl;
            set { this.feedUrl = value; OnPropertyChanged("FeedUrl"); }
        }

        private int contentAmount;

        public int ContentAmount
        {
            get => contentAmount;
            set { this.contentAmount = value; OnPropertyChanged("ContentAmount"); }
        }

        private int categoriesAmount;

        public int CategoriesAmount
        {
            get
            {
                return this.categoriesAmount;
            }

            set { this.categoriesAmount = value; OnPropertyChanged("CategoriesAmount"); }
        }

        private Category selectedCategory;

        public Category SelectedCategory
        {
            get => selectedCategory;
            set { this.selectedCategory = value; OnPropertyChanged("SelectedCategory"); }
        }

        private string contentTitle;

        public string ContentTitle
        {
            get => contentTitle;
            set { contentTitle = value; OnPropertyChanged("ContentTitle"); }
        }

        private string contentUri;

        public string ContentUri
        {
            get => contentUri;
            set { contentUri = value; OnPropertyChanged("ContentUri"); }
        }

        private bool logFeedsEnabled;

        public bool LogFeedsEnabled 
        { 
            get => logFeedsEnabled;
            set { this.logFeedsEnabled = value; OnPropertyChanged(nameof(LogFeedsEnabled)); } 
        }

        private bool logCategoriesEnabled;

        public bool LogCategoriesEnabled 
        { 
            get => logCategoriesEnabled;
            set { logCategoriesEnabled = value; OnPropertyChanged(nameof(LogCategoriesEnabled)); }
        }

        private AddCategoryCommand addCategoryCommand;

        public AddCategoryCommand AddCategoryCommand
        {
            get
            {
                return addCategoryCommand ??
                    (addCategoryCommand = new AddCategoryCommand(this));
            }
        }

        private AddContentCommand addContentCommand;

        public AddContentCommand AddContentCommand
        {
            get
            {
                return this.addContentCommand ?? (addContentCommand = new AddContentCommand(this, async obj =>
                {



                    Content content = new Content()
                    {
                        CategoryId = SelectedCategory.CategoryId,
                        ImageUri = ContentUri,
                        Title = ContentTitle
                    };

                    Realm _realm = Realm.GetInstance();

                    _realm.Write(() => _realm.Add<Content>(content));
                }));
            }
        }

        private BackupDbCommand backupDbCommand;

        public BackupDbCommand BackupDbCommand
        {
            get
            {
                return backupDbCommand ??
                    (backupDbCommand = new BackupDbCommand());
            }
        }

        private EraseEmptyCategoriesCommand eraseEmptyCategoriesCommand;

        public EraseEmptyCategoriesCommand EraseEmptyCategoriesCommand
        {
            get
            {
                return eraseEmptyCategoriesCommand ??
                    (eraseEmptyCategoriesCommand = new EraseEmptyCategoriesCommand());
            }
        }

        private SyncAllContentCommand syncContextCommand;

        public SyncAllContentCommand SyncContextCommand
        {
            get
            {
                return this.syncContextCommand ??
                    (syncContextCommand = new SyncAllContentCommand(this));
            }
        }

        private PostDataToBackEndCommand postNewContentCommand;

        public PostDataToBackEndCommand PostNewContentCommand 
        {
            get 
            {
                return this.postNewContentCommand ?? (this.postNewContentCommand = new PostDataToBackEndCommand());
            }
        }

        private WipeAllDataCommand wipeAllDataCommand;

        public WipeAllDataCommand WipeAllDataCommand 
        {
            get 
            {
                return this.wipeAllDataCommand ?? (this.wipeAllDataCommand = new WipeAllDataCommand());
            }
        }

        private QuitAppCommand closeAppCommand;

        public QuitAppCommand CloseAppCommand 
        {
            get 
            {
                return this.closeAppCommand ?? (this.closeAppCommand = new QuitAppCommand());
            }
        }

        private ParseRemoteContentFeedCommand parseFeedCommand;

        public ParseRemoteContentFeedCommand ParseFeedCommand 
        {
            get => this.parseFeedCommand ?? (this.parseFeedCommand = new ParseRemoteContentFeedCommand(this));
        }

        private DeleteImagesCommand deleteImagesCommand;

        public DeleteImagesCommand DeleteImagesCommand 
        {
            get => this.deleteImagesCommand ?? (this.deleteImagesCommand = new DeleteImagesCommand());
        }

        public SettingsViewModel()
        {
            this.Categories = new ObservableCollection<Category>();

            Realm _realm = Realm.GetInstance();

            this.CategoriesAmount = _realm.All<Category>().Count();
            this.contentAmount = _realm.All<Content>().Count();
            this.logFeedsEnabled = true;
        }

        public void UpdateAllCategories()
        {
            this.Categories.Clear();
            this.Categories = Realm.GetInstance().All<Category>().OrderBy(c => c.Name).ToObservableCollection();
        }
    }
}
