using SaverMaui.ViewModels;
using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class FeedItemChangedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private FeedViewModel viewModel;

        public FeedItemChangedCommand(FeedViewModel vm)
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
                    $"Setting foreground image: {Environment.CurrentImageOnScreen.Id} \r\n");
                    
            }
            catch (Exception)
            {
                NotificationCenterViewModel.GetInstance()?.SendLocalMessage($"Debug:");
            }
        }
    }
}
