using BananasGambler.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananasGambler.ViewModels
{
    internal class MyCardsViewModel : BaseViewModel
    {
        public ObservableCollection<GameCardDto> Cards { get; set; }

        public MyCardsViewModel()
        {
            this.Cards = new ObservableCollection<GameCardDto>();
            this.Cards.Add(new GameCardDto 
            { 
                CardTitle = "Test",
                CostInCredits = 0,
                DateCreated = DateTime.Now,
                Id = 1,
                ImageUri = "http://localhost",
                Rarity = 1
            });

            this.Cards.Add(new GameCardDto
            {
                CardTitle = "Test2",
                CostInCredits = 0,
                DateCreated = DateTime.Now,
                Id = 1,
                ImageUri = "http://localhost",
                Rarity = 1
            });
            this.Cards.Add(new GameCardDto
            {
                CardTitle = "Tes3",
                CostInCredits = 0,
                DateCreated = DateTime.Now,
                Id = 1,
                ImageUri = "http://localhost",
                Rarity = 1
            });
            this.Cards.Add(new GameCardDto
            {
                CardTitle = "Test4",
                CostInCredits = 0,
                DateCreated = DateTime.Now,
                Id = 1,
                ImageUri = "http://localhost",
                Rarity = 1
            });
            this.Cards.Add(new GameCardDto
            {
                CardTitle = "Test5",
                CostInCredits = 0,
                DateCreated = DateTime.Now,
                Id = 1,
                ImageUri = "http://localhost",
                Rarity = 1
            });
            this.Cards.Add(new GameCardDto
            {
                CardTitle = "Test6",
                CostInCredits = 0,
                DateCreated = DateTime.Now,
                Id = 1,
                ImageUri = "http://localhost",
                Rarity = 1
            });
        }
    }
}
