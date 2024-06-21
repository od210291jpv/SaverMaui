namespace BananasGambler.Helpers
{
    internal static class UrlHelper
    {
        internal static string BaseUrl
        {
            get
            {
                //IConfigurationSection section = this.configurationRoot.GetSection(nameof(HttpClientConfiguration));
                //return section.GetValue<string>("BaseUrl");
                return "http://192.168.88.252:5200";
            }
        }

        internal static string LoginUrl = $"{BaseUrl}/Login/Login";

        internal static string GetCards = $"{BaseUrl}/Cards/GetCards";

        internal static string GetUserCards = $"{BaseUrl}/Cards/GetUserCards";

        internal static string BuyAcard(int cardId) 
        {
            return $"{BaseUrl}/Cards/PurchaseACard?cardId={cardId}";
        }

        internal static string CheckInCredits(string code) 
        {
            return $"{BaseUrl}/CheckInCredits/checkin?code={code}";
        }

        internal static string StartGame(int cardId) 
        {
            return $"{BaseUrl}/Game/StartGameSession?bidCardId={cardId}";
        }

        internal static string PlayGame(bool pass, int value, bool cardAsReward) 
        {
            return $"{BaseUrl}/Game/Play?pass={pass}&value={value}&getCardAsReward={cardAsReward}";
        }

        internal static string LogOut = $"{BaseUrl}/Login/Logout";

        internal static string SignUp = $"{BaseUrl}/SignupUser/Signup";

        internal static string GetAllCardCategories = $"{BaseUrl}/GetAllCategories";
    }
}
