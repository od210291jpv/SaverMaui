using Realms;
using SaverMaui.Models;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class DeleteImagesCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Environment.ImagesToDelete.Count > 0;
        }

        public async void Execute(object parameter)
        {
            if (Environment.ImagesToDelete.Count > 0)
            {
                Realm _realm = Realm.GetInstance();

                foreach (var c in Environment.ImagesToDelete)
                {
                    var all = _realm.All<Content>().ToArray();
                    var targetImage = all.Where(i => i.ImageUri.ToString().Contains(c)).FirstOrDefault();
                    if (targetImage is not null) 
                    {
                        _realm.Write(() => _realm.Remove(targetImage));
                    }
                }

                await Application.Current.MainPage.DisplayAlert("Done", $"{Environment.ImagesToDelete.Count} images where deleted", "Ok");
                Environment.ImagesToDelete.Clear();
            }
            else 
            {
                await Application.Current.MainPage.DisplayAlert("Warning", $"No images in list to be deleted", "Ok");
            }
        }
    }
}
