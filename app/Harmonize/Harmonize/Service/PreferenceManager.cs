using Harmonize.Client;
using Harmonize.Kodi;
using Harmonize.Model;

namespace Harmonize.Service;

public class PreferenceManager(
    HarmonizeClient harmonizeClient,
    KodiClient kodiClient
    )
{
    public UserSettings UserSettings { get; private set; } = new()
    {
        Port = Preferences.Default.Get(nameof(UserSettings.Port), 8000),
        DomainName = Preferences.Default.Get(nameof(UserSettings.DomainName), "127.0.0.1"),
        DefaultPageOnLaunch = Preferences.Default.Get(nameof(UserSettings.DefaultPageOnLaunch), "Home"),
        ResetDatabaseOnLaunch = Preferences.Default.Get(nameof(UserSettings.ResetDatabaseOnLaunch), false),
        IncludeMediaControlPage = Preferences.Default.Get(nameof(UserSettings.IncludeMediaControlPage), false),
        KodiDomainName = Preferences.Default.Get(nameof(UserSettings.KodiDomainName), "127.0.0.1"),
        KodiPort = Preferences.Default.Get(nameof(UserSettings.KodiPort), 8080),
        KodiApiUserName = Preferences.Default.Get(nameof(UserSettings.KodiApiUserName), ""),
        KodiApiPasword = Preferences.Default.Get(nameof(UserSettings.KodiApiPasword), ""),
        HarmonizeUserName = Preferences.Default.Get(nameof(UserSettings.HarmonizeUserName), ""),
        HarmonizePassword = Preferences.Default.Get(nameof(UserSettings.HarmonizePassword), ""),
        UrlPrefix = Preferences.Default.Get(nameof(UserSettings.UrlPrefix), ""),
        UseHttps = Preferences.Default.Get(nameof(UserSettings.UseHttps), false),
    };

    internal PreferenceManager SetUserSetttings(UserSettings userSettings)
    {
        UserSettings = userSettings;

        Preferences.Default.Set(nameof(UserSettings.DomainName), userSettings.DomainName);
        Preferences.Default.Set(nameof(UserSettings.DefaultPageOnLaunch), userSettings.DefaultPageOnLaunch);
        Preferences.Default.Set(nameof(UserSettings.Port), userSettings.Port);
        Preferences.Default.Set(nameof(UserSettings.ResetDatabaseOnLaunch), userSettings.ResetDatabaseOnLaunch);
        Preferences.Default.Set(nameof(UserSettings.IncludeMediaControlPage), userSettings.IncludeMediaControlPage);
        Preferences.Default.Set(nameof(UserSettings.KodiDomainName), userSettings.KodiDomainName);
        Preferences.Default.Set(nameof(UserSettings.KodiPort), userSettings.KodiPort);
        Preferences.Default.Set(nameof(UserSettings.KodiApiUserName), userSettings.KodiApiUserName);
        Preferences.Default.Set(nameof(UserSettings.KodiApiPasword), userSettings.KodiApiPasword);
        Preferences.Default.Set(nameof(UserSettings.HarmonizeUserName), userSettings.HarmonizeUserName);
        Preferences.Default.Set(nameof(UserSettings.HarmonizePassword), userSettings.HarmonizePassword);
        Preferences.Default.Set(nameof(UserSettings.UrlPrefix), userSettings.UrlPrefix);
        Preferences.Default.Set(nameof(UserSettings.UseHttps), userSettings.UseHttps);

        harmonizeClient
            .SetPort(UserSettings.Port)
            .SetHostName(UserSettings.DomainName)
            .SetUrlPrefix(UserSettings.UrlPrefix)
            .SetUseHttps(UserSettings.UseHttps)
            .SetCredentials(UserSettings.HarmonizeUserName, UserSettings.HarmonizePassword)
            ;

        kodiClient
            .SetHostName(userSettings.KodiDomainName)
            .SetPort(userSettings.KodiPort)
            .SetUserName(userSettings.KodiApiUserName)
            .SetPassword(userSettings.KodiApiPasword)
            ;

        return this;
    }
}
