using BananasGamblerBackend.Database;
using BananasGamblerBackend.Database.Models;
using BananasGamblerBackend.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BananasGamblerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardsController : Controller
    {
        private ApplicationContext database;

        public CardsController(ApplicationContext context)
        {
            this.database = context;
        }

        [HttpGet("GetCards")]
        public GameCardDto[] Index()
        {
            var allCards = this.database.GameCards;

            var result = new List<GameCardDto>();

            foreach (var card in allCards) 
            {
                result.Add(new GameCardDto() 
                {
                    CardTitle = card.CardTitle,
                    CostInCredits = card.CostInCredits,
                    DateCreated = card.DateCreated,
                    Id = card.Id,
                    ImageUri = card.ImageUri,
                    Rarity = card.Rarity
                });
            }

            return result.ToArray();
        }

        [HttpPost("GetUserCards")]
        public async Task<GameCard[]> GetUserCards(LoginDto loginData) 
        {
            var user = await this.database.Users.FirstOrDefaultAsync(u => u.Login == loginData.Login && u.Password == loginData.Password);

            if (user == null)
            {
                Array.Empty<GameCard>();
            }

            var result = this.database.GameCards.Where(c => c.Users.Select(u => u.Id).Contains(user!.Id)).ToArray();
            return result;
        }

        [HttpPost("PurchaseACard")]
        public async Task<IActionResult> PurchaseACard(LoginDto loginData, int cardId) 
        {
            var user = await this.database.Users.FirstOrDefaultAsync(u => u.Login == loginData.Login && u.Password == loginData.Password);

            if (user == null) 
            {
                return Unauthorized();
            }

            var card = await this.database.GameCards.FirstOrDefaultAsync(c => c.Id == cardId);
            if (card == null) 
            {
                return NotFound("Card not found");
            }

            if (user.Credits < card.CostInCredits) 
            {
                return BadRequest("Not enough credits!");
            }

            user.Credits -= card.CostInCredits;
            card.Users.Add(user);

            await this.database.SaveChangesAsync();
            return Ok();
        }
    }
}
