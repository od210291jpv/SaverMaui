using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class FavoriteContentFeedItemChangedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private FavoriteContentViewModel viewModel;

        public FavoriteContentFeedItemChangedCommand(FavoriteContentViewModel vm)
        {
            this.viewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                Environment.CurrentImageOnScreen = this.viewModel.CurrentContent;

                NotificationCenterViewModel.GetInstance()?.SendLocalMessage($"Debug: " +
                    $"Setting fav foreground image: {Environment.CurrentImageOnScreen.Id} \r\n");

            }
            catch (Exception)
            {
                NotificationCenterViewModel.GetInstance()?.SendLocalMessage($"Debug:");
            }
        }
    }
}
