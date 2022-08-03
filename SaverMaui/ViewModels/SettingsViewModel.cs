using Realms;
using SaverMaui.Commands;
using SaverMaui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaverMaui.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string hostIpAddress;

        public string HostIpAddress
        {
            get { return hostIpAddress; }
            set
            {
                hostIpAddress = value;
                OnPropertyChanged("HostIpAddress");
            }
        }

        public ObservableCollection<Category> Categories { get; set; }

        private string newCategoryName;

        public string NewCategoryName
        {
            get => newCategoryName;
            set { this.newCategoryName = value; OnPropertyChanged("NewCategoryName"); }
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
            get => categoriesAmount;
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
                return this.addContentCommand ?? (addContentCommand = new AddContentCommand(this, obj =>
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


        public SettingsViewModel()
        {
            this.Categories = new ObservableCollection<Category>();

            Realm _realm = Realm.GetInstance();

            var allCategories = _realm.All<Category>();

            foreach (var cat in allCategories)
            {
                Categories.Add(cat);
            }

            this.CategoriesAmount = allCategories.ToArray().Length;
            this.ContentAmount = _realm.All<Content>().ToArray().Length;
        }
    }
}
