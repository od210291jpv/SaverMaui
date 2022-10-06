using SaverMaui.Commands;
using SaverMaui.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    internal class MostPopularCategoriesViewModel : BaseViewModel
    {
        private Category selectedCategory;

        public Category SelectedCategory 
        {
            get { return selectedCategory; }
            set 
            {
                selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public ObservableCollection<Category> MostPopularCategories { get; set; }

        public ICommand NavigateToCategoryCommand 
        {
            get;
        }

        public ICommand GetMostPopularCategories 
        {
            get;
        }

        public MostPopularCategoriesViewModel()
        {
            this.NavigateToCategoryCommand = new NavigateToPopularCategoryCommand(this);
            this.GetMostPopularCategories = new GetMostPopularCategoriesCommand(this);

            this.MostPopularCategories = new ObservableCollection<Category>();

            this.GetMostPopularCategories.Execute(this);
        }
    }
}
