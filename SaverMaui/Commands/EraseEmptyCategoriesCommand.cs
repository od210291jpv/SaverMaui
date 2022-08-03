using Realms;
using SaverMaui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class EraseEmptyCategoriesCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            Realm _realm = Realm.GetInstance();
            Content[] allRelatedContent = _realm.All<Content>().ToArray();
            Category[] allRelatedCategories = _realm.All<Category>().ToArray();
            var categoriesToDelete = allRelatedCategories.Where(c => allRelatedContent.Count(rc => rc.CategoryId.Equals(c.CategoryId)) == 0).ToArray();
            if (categoriesToDelete.Length > 0)
            {
                bool answer = await Application.Current.MainPage.DisplayAlert("Caution", $"Would you like to delete {categoriesToDelete.Length} categories?", "Yes", "No");

                if (answer == true)
                {
                    foreach (var cat in categoriesToDelete)
                    {
                        _realm.Write(() => _realm.Remove(cat));
                    }

                    await Application.Current.MainPage.DisplayAlert("Done", $"Categories where deleted", "Ok");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Done", $"Operation aborted", "Ok");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Done", $"No empty categories where found!", "Ok");
            }
        }
    }

}
