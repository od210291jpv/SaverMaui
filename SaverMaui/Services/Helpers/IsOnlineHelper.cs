namespace SaverMaui.Services.Helpers
{
    internal static class IsOnlineHelper
    {
        internal static bool IsOnline
        {
            get 
            {
                NetworkAccess accessType = Connectivity.Current.NetworkAccess;

                if (accessType == NetworkAccess.Internet)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
