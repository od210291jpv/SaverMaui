using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Realms;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    public class AddContentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> execute;
        private SettingsViewModel viewModel;

        public AddContentCommand(SettingsViewModel vm, Action<object> _execute)
        {
            this.execute = _execute;
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            Realm _realm = Realm.GetInstance();

            if (_realm != null)
            {
                return true;
            }
            return false;
        }

        public async void Execute(object parameter)
        {
            this.execute(parameter);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"New content added!", ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
            this.viewModel.ContentUri = "";

            SettingsViewModel.GetInstance().ContentAmount += 1;


            if (FeedViewModel.Instance != null) 
            {
                FeedViewModel.Instance.ContentCollection.Clear();

                Realm _realm = Realm.GetInstance();
                Content[] allRelatedContent = _realm.All<Content>().ToArray();

                ObservableCollection<ImageRepresentationElement> allFeed = new();

                foreach (var cat in allRelatedContent.ToArray().Reverse())
                {
                    allFeed.Add(new ImageRepresentationElement()
                    {
                        CategoryId = cat.CategoryId.Value,
                        Name = cat.Title,
                        Source = cat.ImageUri,
                        IsFavorite = cat.IsFavorite
                    });
                }

                FeedViewModel.Instance.ContentCollection = allFeed;
            }
        }
    }

}
