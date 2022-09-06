using Realms;
using SaverMaui.Models;

namespace SaverMaui
{
    public static class Environment
    {
        public static class SahredData
        {
            public static Category currentCategory;

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
