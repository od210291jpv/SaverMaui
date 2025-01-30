using Realms;
using SaverMaui.Custom_Elements;
using SaverMaui.Models;
using SaverMaui.Services.Contracts.Profile;
using SaverMaui.Services.Contracts.Category;

namespace SaverMaui
{
    public static class Environment
    {
        public static string Login;

        public static string Password;

        public static Guid ProfileId;

        public static int ProfileIntId;

        public static LoginResponse ProfileData;

        public static KeyValuePair<string, SearchResult[]> CurrectSearchResultCategory;

        public static ImageRepresentationElement CurrentImageOnScreen;

        public static string CurrectSearchResultItem  = "";

        public static List<string> ImagesToDelete = new List<string>();

        public static bool SearchResultsResfresh { get; internal set; }

        public static class SahredData
        {
            public static CategoryDto currentCategory;

            public static WebContent currentWebContent;
        }

        public static class SharedSettings
        {
            public static SharedSetting ReloadPageOnAppearing
            {
                get
                {
                    Realm _realm = Realm.GetInstance();

                    var allSettings = _realm.All<SharedSetting>().ToArray();

                    if (!allSettings.Any(s => s.Name == nameof(ReloadPageOnAppearing)))
                    {
                        SharedSetting settings = new SharedSetting() { Name = nameof(ReloadPageOnAppearing), Param = false };

                        _realm.Write(() => _realm.Add(settings));
                        return settings;
                    }

                    return allSettings.Single(s => s.Name == nameof(ReloadPageOnAppearing));
                }

                set
                {
                    Realm _realm = Realm.GetInstance();

                    _realm.Write(() => _realm.Add(value));
                }
            }
        }
    }
}
