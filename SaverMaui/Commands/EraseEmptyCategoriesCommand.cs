using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Models;
using SaverMaui.ViewModels;
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
                        await _realm.WriteAsync(() => _realm.Remove(cat));
                    }

                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    var toast = Toast.Make($"Categories where deleted", ToastDuration.Short, 14);
                    await toast.Show(cancellationTokenSource.Token);
                }
                else
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    var toast = Toast.Make($"Operation aborted", ToastDuration.Short, 14);
                    await toast.Show(cancellationTokenSource.Token);
                }
            }
            else
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make($"No empty categories where found", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
            }
        }
    }

}
