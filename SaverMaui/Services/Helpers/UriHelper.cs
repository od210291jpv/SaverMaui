namespace SaverMaui.Services.Helpers
{
    public static class UriHelper
    {
        public static string BaseIp = "192.168.88.252";
        //public static string BaseIp = "127.0.0.1";

        //public static int Port = 5000;
        public static int Port = 80;
        public static int SignalRPort = 80;

        public static string ImageRecognitionHost = "192.168.88.20";
        public static string ImageRecognitionApi = $"http://{BaseIp}/RecognizeImage?imageUri=";

        public static string SendNotificationUrl = $"http://{BaseIp}:{SignalRPort}/notify";

        public static string BaseAddress = $"http://{BaseIp}:{Port}";

        public static string GetAllContent 
        {
            get => $"{BaseAddress}/GetAllContent";
        }

        public static string GetPaginatedContent(short page = 0, short size = 200) 
        {
            return $"{BaseAddress}/GetAllContent/GetAllContentPaged?page={page}&pageSize={size}";
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

        public static string GetLoginStatus(string login, string password) 
        {
            return $"{BaseAddress}/Login/GetLoginStatus?login={login}&password={password}";
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

        public static string DeleteContent(int contentId) 
        {
            return $"{BaseAddress}/SyncContent/DeleteContent?contentId={contentId}";
        }

        public static string SearchResults 
        {
            get => $"{BaseAddress}/GetAllContent/searchResults";
        }

        public static string SearchContent(string keyword) 
        {
            return $"http://192.168.88.252:5011/Parse?keyword={keyword}";    
        }

        public static string CleanSearchResults 
        {
            get => $"{BaseAddress}/GetAllContent/CleanResults";
        }

        public static string RateContent(int contentId, short rating, int profileId) 
        {
            return $"{BaseAddress}/Content/UpdateContentRating?contentId={contentId}&rating={rating}&profileId={profileId}";
        }

        public static string GetRatedContent(short rate = 0) 
        {
            return $"{BaseAddress}/Content/GetRatedContent?rate={rate}";
        }

        public static string GetCategoryContent(Guid categoryId) 
        {
            return $"{BaseAddress}/GetCategories/categoryContent?categoryId={categoryId}";
        }

        public static string ProfileInfo(string login, string password) 
        {
            return $"{BaseAddress}/Login/ProfileInfo?login={login}&password={password}";
        }
    }
}
