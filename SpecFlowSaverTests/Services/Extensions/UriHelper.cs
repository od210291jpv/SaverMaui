﻿namespace SpecFlowSaverTests.Services.Extensions
{
    internal static class UriHelper
    {
        public static string BaseIp = "127.0.0.1";
        //public static string BaseIp = "192.168.0.114";

        public static int Port = 5000;
        public static int SignalRPort = 5000;

        public static string SendNotificationUrl = $"http://{BaseIp}:{SignalRPort}/notify";

        public static string BaseAddress = $"http://{BaseIp}:{Port}";

        public static string GetAllContent
        {
            get => $"{BaseAddress}/GetAllContent";
        }

        public static string CreateCategory
        {
            get => $"{BaseAddress}/CreateCategory/create";
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

        public static string DeleteCategory(Guid categoryId) 
        {
            return $"{BaseAddress}/RemoveCategory/delete?categoryId={categoryId}";
        }
    }
}