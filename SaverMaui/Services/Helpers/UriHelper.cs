namespace SaverMaui.Services.Helpers
{
    public static class UriHelper
    {
        public static string BaseIp = "192.168.88.252";
        //public static string BaseIp = "localhost";

        public static int Port = 80;
        public static int SignalRPort = 80;

        public static string SendNotificationUrl = $"http://{BaseIp}:{SignalRPort}/notify";

        public static string BaseAddress = $"http://{BaseIp}:{Port}";

        public static string GetAllContent 
        {
            get => $"{BaseAddress}/GetAllContent";
        }

        public static string GetAllCategories 
        {
            get => $"{BaseAddress}/GetCategories";
        }

        public static string PostContent 
        {
            get => $"{BaseAddress}/SyncContent";
        }

        public static string UpdateStatistics 
        {
            get => $"{BaseAddress}/UpdateCategoryStatistics";
        }

        public static string CategoriesStatistics 
        {
            get => $"{BaseAddress}/GetCategoryStatistics";
        }

        public static string GetMostPopularCategories(int limit) 
        {
            return $"{BaseAddress}/GetCategories?limit={limit}";
        }

        public static string Login(string login, string password) 
        {
            return $"{BaseAddress}/Login/Login?login={login}&password={password}";
        }

        public static string Logout(string login, string password) 
        {
            return $"{BaseAddress}/Login/Logout?login={login}&password={password}";
        }

        public static string GetLoginStatus(string login) 
        {
            return $"{BaseAddress}/Login/GetLoginStatus?login={login}";
        }

        public static string RegisterUser 
        {
            get => $"{BaseAddress}/RegisterUserProfile/RegisterUserProfile";
        }

        public static string Ping 
        {
            get => $"{BaseAddress}/Ping/ping";
        }

        public static string AddVideo
        {
            get => $"{BaseAddress}/VideoContent";
        }

        public static string GetUserVideo(Guid userId) 
        {
            return $"{BaseAddress}/VideoContent?userId={userId}";
        }
    }
}
