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
        const string DomainNameKey = nameof(DomainNameKey);
        const string DefaultPageOnLaunchKey = nameof(DefaultPageOnLaunchKey);
        const string PortKey = nameof(PortKey);
        public UserSettings UserSettings { get; private set; }
        public PreferenceManager()
        {
            UserSettings = new()
            {
                Port = Preferences.Default.Get(PortKey, 8000),
                DomainName = Preferences.Default.Get(DomainNameKey, "127.0.0.1"),
                DefaultPageOnLaunch = Preferences.Default.Get(DefaultPageOnLaunchKey, "Home"),
            };
        }
        internal PreferenceManager SetUserSetttings(UserSettings userSettings)
        {
            UserSettings = userSettings;

            Preferences.Default.Set(DomainNameKey, userSettings.DomainName);
            Preferences.Default.Set(DefaultPageOnLaunchKey, userSettings.DefaultPageOnLaunch);
            Preferences.Default.Set(PortKey, userSettings.Port);

            return this;
        }
    }
}
