using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make($"{Environment.ImagesToDelete.Count} images where deleted", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
                Environment.ImagesToDelete.Clear();
            }
            else 
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make($"No images in list to be deleted", ToastDuration.Short, 14);
                await toast.Show(cancellationTokenSource.Token);
            }
        }
    }
}
