using Realms;
using SaverMaui.Commands;
using SaverMaui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class CategoriesViewModel : BaseViewModel
    {
        public ObservableCollection<Category> Categories { get; set; }

        private Category selectedCategory;

        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }

        private NavigateToPageCommand navigateToFeedItemCommand;

        public NavigateToPageCommand NavigateToFeedItemCommand
        {
            get
            {
                return navigateToFeedItemCommand ??
                    (navigateToFeedItemCommand = new NavigateToPageCommand(this));
            }
        }

        public ICommand AddFavoriteCategoryCmd
        {
            get;
        }

        public CategoriesViewModel()
        {

            this.AddFavoriteCategoryCmd = new AddFavoriteCategoryCommand(this);

            this.Categories = new ObservableCollection<Category>();

            Realm _realm = Realm.GetInstance();
            var allCategories = _realm.All<Category>();

            foreach (var cat in allCategories)
            {
                Categories.Add(cat);
            }

            this.PropertyChanged += OnCurrentCategoryChanged;
        }

        public void OnCurrentCategoryChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(this.SelectedCategory)))
            {
                Environment.SahredData.currentCategory = this.SelectedCategory;
            }
        }
    }
}
