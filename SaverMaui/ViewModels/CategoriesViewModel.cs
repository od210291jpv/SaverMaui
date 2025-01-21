using CommunityToolkit.Maui.Core.Extensions;
using Realms;
using SaverMaui.Commands;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts.Category;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class CategoriesViewModel : BaseViewModel
    {
        public static CategoriesViewModel? Instance { get; private set; }

        private ObservableCollection<CategoryDto> categories;

        public ObservableCollection<CategoryDto> Categories 
        {
            get { return this.categories; }
            set
            {               
                this.categories = value;
                OnPropertyChanged("Categories");
            }
        }

        private CategoryDto selectedCategory;

        public CategoryDto SelectedCategory
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
                return navigateToFeedItemCommand ??= new NavigateToPageCommand(this);
            }
        }

        public ICommand AddFavoriteCategoryCmd
        {
            get;
        }

        public CategoriesViewModel()
        {
            this.Categories = new ObservableCollection<CategoryDto>();
            this.categories = new ObservableCollection<CategoryDto>();
            this.AddFavoriteCategoryCmd = new AddFavoriteCategoryCommand(this);

            this.selectedCategory = new CategoryDto();
            this.SelectedCategory = new CategoryDto();

            Instance = this;
        }

        public void OnCurrentCategoryChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(this.SelectedCategory)))
            {
                Environment.SahredData.currentCategory = this.SelectedCategory;
            }
        }

        public void UpdateAllCategories() 
        {
            this.Categories.Clear();

            
        }
    }
}
