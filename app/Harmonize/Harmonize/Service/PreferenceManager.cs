using Harmonize.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Service
{
    public class PreferenceManager
    {
        public UserSettings UserSettings { get; private set; }
        public PreferenceManager()
        {
            UserSettings = new()
            {
                Port = Preferences.Default.Get(nameof(UserSettings.Port), 8000),
                DomainName = Preferences.Default.Get(nameof(UserSettings.DomainName), "127.0.0.1"),
                DefaultPageOnLaunch = Preferences.Default.Get(nameof(UserSettings.DefaultPageOnLaunch), "Home"),
                ResetDatabaseOnLaunch = Preferences.Default.Get(nameof(UserSettings.ResetDatabaseOnLaunch), false),
                IncludeMediaControlPage = Preferences.Default.Get(nameof(UserSettings.IncludeMediaControlPage), false),
            };
        }
        internal PreferenceManager SetUserSetttings(UserSettings userSettings)
        {
            UserSettings = userSettings;

            Preferences.Default.Set(nameof(UserSettings.DomainName), userSettings.DomainName);
            Preferences.Default.Set(nameof(UserSettings.DefaultPageOnLaunch), userSettings.DefaultPageOnLaunch);
            Preferences.Default.Set(nameof(UserSettings.Port), userSettings.Port);
            Preferences.Default.Set(nameof(UserSettings.ResetDatabaseOnLaunch), userSettings.ResetDatabaseOnLaunch);
            Preferences.Default.Set(nameof(UserSettings.IncludeMediaControlPage), userSettings.IncludeMediaControlPage);

            return this;
        }
    }
}
