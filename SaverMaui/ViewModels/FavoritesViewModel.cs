using Realms;
using SaverMaui.Commands;
using SaverMaui.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class FavoritesViewModel : BaseViewModel
    {
        public ObservableCollection<Category> FavoriteCategories { get; set; }

        public ObservableCollection<Content> AllFavoriteContent { get; set; }

        public ObservableCollection<Content> FavoriteContent { get; set; }

        private Category selectedFavoriteCategory;

        public Category SelectedFavoriteCategory
        {
            get { return selectedFavoriteCategory; }
            set
            {
                selectedFavoriteCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }

        public ICommand NavigateToFeedItemCommand
        {
            get;
        }

        public ICommand NavigateToFavoriteCategoriesCommand
        {
            get;
        }

        public ICommand NavigateToFavoriteContentCommand 
        {
            get;
        }

        public ICommand NavigateToPersonalFeedCommand 
        {
            get;
        }

        public FavoritesViewModel()
        {
            this.NavigateToFeedItemCommand = new NavigateToFavoriteCategoriesPageCommand(this);
            this.NavigateToFavoriteCategoriesCommand = new NavigateToFavoriteCategoriesCommand(this);
            this.FavoriteCategories = new ObservableCollection<Category>();
            this.FavoriteContent = new ObservableCollection<Content>();
            this.AllFavoriteContent = new ObservableCollection<Content>();
            this.NavigateToPersonalFeedCommand = new NavigateToPersonalFeedCommand(this);
            this.NavigateToFavoriteContentCommand = new NavigateToFavoriteContentCommand();

            Realm _realm = Realm.GetInstance();
            var allFavCategories = _realm.All<Category>().ToArray().Where(c => c.IsFavorite == true);

            foreach (var fc in allFavCategories)
            {
                FavoriteCategories.Add(fc);
            }

            var filteredCategories = allFavCategories.Where(c =>
            this.FavoriteCategories.Where(fc =>
            fc.IsFavorite == true).Select(fc =>
            fc.CategoryId).ToArray().Contains(c.CategoryId) == true).ToArray();

            var allContent = _realm.All<Content>().ToArray();

            foreach (var c in allFavCategories)
            {
                foreach (var ctnt in allContent.Where(cn => cn.CategoryId == c.CategoryId))
                {
                    this.AllFavoriteContent.Add(ctnt);
                }
            }
        }
    }
}
