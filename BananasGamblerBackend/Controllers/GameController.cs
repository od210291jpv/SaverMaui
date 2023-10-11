using BananasGamblerBackend.Database;
using BananasGamblerBackend.Database.Models;
using BananasGamblerBackend.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace BananasGamblerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private ApplicationContext database;
        private ConnectionMultiplexer redis;
        private IDatabase redisDb;

        public GameController(ApplicationContext context)
        {
            this.database = context;
            this.redis = ConnectionMultiplexer.Connect("192.168.88.252:6379");// fix, get from config
            this.redisDb = redis.GetDatabase();
        }

        [HttpPost("StartGameSession")]
        public async Task<int> Index(LoginDto loginData, int bidCardId)
        {
            var user = await this.database.Users.FirstOrDefaultAsync(u => u.Login == loginData.Login && u.Password == loginData.Password);

            if (user == null) 
            {
                return 0;
            }

            var sessionId = user.Id.ToString();
            short value = (short)new Random().Next(1, 21);

            this.redisDb.StringSet(sessionId, $"{value},{bidCardId}");

            return int.Parse(sessionId);
        }

        [HttpPost("Play")]
        public async Task<GameResultDto> Play(LoginDto loginData, bool pass, short value = 0, bool getCardAsReward = false) 
        {
            var user = await this.database.Users.FirstOrDefaultAsync(u => u.Login == loginData.Login && u.Password == loginData.Password);

            if (user == null)
            {
                return new GameResultDto() { Result = "User not found!" };
            }

            var hiddenValue = this.redisDb.StringGet(user.Id.ToString()).ToString();
            var hiddenNumber = short.Parse(hiddenValue.Split(',').First());
            var cardId = int.Parse(hiddenValue.Split(',')[1]);
            var card = this.database.GameCards.SingleOrDefault(c => c.Id == cardId);

            if (pass == true)
            {
                if (value < hiddenNumber)
                {
                    if ((decimal)value > (((decimal)hiddenNumber / 100m) * 80m))
                    {
                        user.Credits += value;
                        await this.database.SaveChangesAsync();
                        this.redisDb.StringGetDelete(user.Id.ToString());

                        return new GameResultDto() 
                        {
                            Result = $"Hidden value was {hiddenNumber}, your value {value}.\r\n Parial win, you get {value} credits",
                            Rewards = new RewardsDto() { NewIncome = value }
                        };
                    }
                    else
                    {
                        this.redisDb.StringGetDelete(user.Id.ToString());
                        return new GameResultDto()
                        {
                            Result = $"Hidden value was {hiddenNumber}, your value {value}.\r\n No win"
                        };
                    }
                }
            }

            if (value < hiddenNumber && pass == false)
            {
                if ((hiddenNumber - value) >= ((hiddenNumber/100)*80))
                {
                    await this.database.SaveChangesAsync();

                    return new GameResultDto()
                    {
                        Result = $"You put {value} wich is still less than hidden number. Continue playing",
                    };
                }
            }

            if (value > hiddenNumber)
            {

                if (card != null)
                {
                    var cardT = this.database.GameCards.Include(c => c.Users).Single(c => c.Id == cardId);
                    cardT?.Users.Remove(user);

                    await this.database.SaveChangesAsync();

                    this.redisDb.StringGetDelete(user.Id.ToString());


                    return new GameResultDto()
                    {
                        Result = $"Hidden value was {hiddenNumber}, your value {value}.\r\n You loose your card."
                    };
                }
            }

            if (value == hiddenNumber)
            {
                if (getCardAsReward == true)
                {
                    var randomCards = this.database.GameCards.Where(c => c.Users.Select(u => u.Id).Contains(user.Id) == false).ToArray();
                    var randomCard = randomCards[new Random().Next(0, randomCards.Count() - 1)];
                    randomCard.Users.Add(user);

                    var userCardCategories = user.Cards.Select(c => c.CategoryId).ToArray();

                    string resultMessage = string.Empty;

                    if (userCardCategories.Contains(randomCard.CategoryId) == true)
                    {
                        resultMessage = $"Hidden value was {hiddenNumber}, your value {value}.\r\n Congrats, you win! you get new card in stack! The reward is: {decimal.Round((100 + card.CostInCredits), 2)} credits and new card!";
                    }
                    else 
                    {
                        resultMessage = $"Hidden value was {hiddenNumber}, your value {value}.\r\n Congrats, you win! you get {decimal.Round((100 + card.Rarity), 2)} credits and new card!";
                    }

                    RewardsDto reward = new();
                    if (userCardCategories.Contains(randomCard.CategoryId) == true)
                    {
                        user.Credits += 100 + card.CostInCredits;
                        await this.database.SaveChangesAsync();

                        reward.NewIncome = 100 + card.CostInCredits;
                        reward.RewardCard = randomCard.Id;
                    }
                    else 
                    {
                        user.Credits += 100 + card.Rarity;
                        await this.database.SaveChangesAsync();

                        reward.NewIncome = 100 + card.Rarity;
                        reward.RewardCard = randomCard.Id;
                    }




                    this.redisDb.StringGetDelete(user.Id.ToString());

                    return new GameResultDto()
                    {
                        Result = resultMessage,
                        Rewards = reward
                    };
                }
                else
                {
                    user.Credits += (100 + card.CostInCredits);
                    await this.database.SaveChangesAsync();

                    this.redisDb.StringGetDelete(user.Id.ToString());

                    return new GameResultDto()
                    {
                        Result = $"Hidden value was {hiddenNumber}, your value {value}.\r\n Congrats, you win! you get {100 + card.CostInCredits + card.Rarity} credits!",
                        Rewards = new RewardsDto() {NewIncome = 100 + card.CostInCredits + card.Rarity }
                    };
                }
                // you win +100 credits + card price * 2
                // if get card as reward: card price and random card

                
            }

            return new GameResultDto() { Result= "Continue playing" };
        }
    }
}
