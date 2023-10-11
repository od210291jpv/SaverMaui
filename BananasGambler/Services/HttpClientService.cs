using BananasGambler.DTO;
using BananasGambler.Helpers;
using RestSharp;
using System.Net;

namespace BananasGambler.Services
{
    internal class HttpClientService
    {
        public static HttpClientService Instance;

        public HttpClientService()
        {
        }

        public static HttpClientService GetInstance() 
        {
            Instance ??= new HttpClientService();

            return Instance;
        }

        public LoginResponseDto Login(LoginRequestDto requestModel) 
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(UrlHelper.LoginUrl, Method.Post).AddJsonBody(requestModel);

            return client.Post<LoginResponseDto>(request);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto requestModel) 
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(UrlHelper.LoginUrl, Method.Post).AddJsonBody(requestModel);

            return await client.PostAsync<LoginResponseDto>(request);
        }

        public async Task<GameCardDto[]> GetAllCardsAsync() 
        {
            RestClient client = new RestClient();
            return await client.GetAsync<GameCardDto[]>(new RestRequest(UrlHelper.GetCards, Method.Get));
        }

        public GameCardDto[] GetUserCards(LoginRequestDto requestModel)
        {
            RestClient client = new RestClient();
            return client.Post<GameCardDto[]>(new RestRequest(UrlHelper.GetUserCards, Method.Post).AddJsonBody(requestModel));
        }

        public async Task<GameCardDto[]> GetUserCardsAsync(LoginRequestDto requestModel) 
        {
            RestClient client = new RestClient();
            return await client.PostAsync<GameCardDto[]>(new RestRequest(UrlHelper.GetUserCards, Method.Post).AddJsonBody(requestModel));
        }

        public async Task<HttpStatusCode> BuyACardAsync(LoginRequestDto requestModel, int cardId) 
        {
            RestClient client = new RestClient();
            var response = await client.PostAsync(new RestRequest(UrlHelper.BuyAcard(cardId), Method.Post).AddJsonBody(requestModel));
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> CheckInCreditsAsync(LoginRequestDto requestModel, string checkInCode) 
        {
            RestClient client = new RestClient();
            var response = await client.PostAsync(new RestRequest(UrlHelper.CheckInCredits(checkInCode), Method.Post).AddJsonBody(requestModel));
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> StartGameAsync(LoginRequestDto requestModel, int bidCardId) 
        {
            RestClient client = new RestClient();
            var responce = await client.PostAsync(new RestRequest(UrlHelper.StartGame(bidCardId), Method.Post).AddJsonBody(requestModel));
            return responce.StatusCode;
        }

        public async Task<GameResultDto> PlayGameAsync(LoginRequestDto requestModel, bool pass, int value, bool getCardAsReward) 
        {
            RestClient client = new RestClient();
            return await client.PostAsync<GameResultDto>(new RestRequest(UrlHelper.PlayGame(pass, value, getCardAsReward), Method.Post).AddJsonBody(requestModel));
        }
    }
}
