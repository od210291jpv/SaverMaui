using BananasGambler.Commands;
using BananasGambler.ViewModels;
using System.Windows.Input;

namespace BananasGambler.Views;

public partial class MyCardsPage : ContentPage
{
	public MyCardsPage()
	{
		InitializeComponent();
	}

    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        DTO.GameCardDto currentCard = MyCardsViewModel.Instance?.CurrentCard;

        if (PlayGameViewModel.Instance != null) 
        {
            PlayGameViewModel.Instance.BidCard = currentCard;
        }

        ICommand playCommand = new PlayGameCommand(PlayGameViewModel.Instance);
        if (playCommand.CanExecute(null) == true) 
        {
            playCommand.Execute(null);
        }
    }
}