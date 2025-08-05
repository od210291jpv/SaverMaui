namespace SaverBackendApiClient.Helpers
{
    public static class UriHelper
    {
        public static string RegisterProfile 
        {
            get => "/RegisterUserProfile/RegisterUserProfile";
        }

        public static string GetPagedContent 
        {
            get => "/GetAllContent/GetAllContentPaged?page=0&pageSize=50";
        }
    }
}
