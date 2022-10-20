using Realms;
using SaverMaui.Models;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class BackupDbCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var result = await FilePicker.PickAsync();

            if (result != null)
            {
                Realm _realm = Realm.GetInstance();
                Content[] allRelatedContent = _realm.All<Content>().ToArray();
                Category[] allRelatedCategories = _realm.All<Category>().ToArray();

                string sb = "";

                foreach (Category cat in allRelatedCategories)
                {
                    var filteredContent = allRelatedContent.Where(c => c.CategoryId.Equals(cat.CategoryId)).ToArray();

                    sb += $"{cat.Name}\r\n";
                    foreach (var c in filteredContent)
                    {
                        sb += $"{c.ImageUri}\r\n";
                    }
                }

                File.WriteAllText(result.FullPath, sb);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error!", $"Please select path to an empty backup file!", "Ok");
            }
        }
    }

}
