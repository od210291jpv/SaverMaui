using Realms;
using SaverMaui.Commands;
using SaverMaui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaverMaui.ViewModels
{
    public class FavoritesViewModel : BaseViewModel
    {
        public ObservableCollection<Category> FavoriteCategories { get; set; }


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

        public FavoritesViewModel()
        {
            this.NavigateToFeedItemCommand = new NavigateToFavoriteCategoriesPageCommand(this);
            this.NavigateToFavoriteCategoriesCommand = new NavigateToFavoriteCategoriesCommand(this);
            this.FavoriteCategories = new ObservableCollection<Category>();
            this.FavoriteContent = new ObservableCollection<Content>();

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

        }
    }
}
