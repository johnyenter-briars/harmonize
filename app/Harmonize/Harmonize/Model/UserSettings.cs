namespace Harmonize.Model;

public class UserSettings : NotifyPropertyChangedBase
{
    private string domainName = string.Empty;
    public required string DomainName
    {
        get => domainName;
        set => SetProperty(ref domainName, value);
    }

    private int port;
    public required int Port
    {
        get => port;
        set => SetProperty(ref port, value);
    }

    private string defaultPageOnLaunch = string.Empty;
    public required string DefaultPageOnLaunch
    {
        get => defaultPageOnLaunch;
        set => SetProperty(ref defaultPageOnLaunch, value);
    }

    private bool resetDatabaseOnLaunch;
    public required bool ResetDatabaseOnLaunch
    {
        get => resetDatabaseOnLaunch;
        set => SetProperty(ref resetDatabaseOnLaunch, value);
    }

    private bool includeMediaControlPage;
    public required bool IncludeMediaControlPage
    {
        get => includeMediaControlPage;
        set => SetProperty(ref includeMediaControlPage, value);
    }

    #region Kodi
    private string kodiDomainName = string.Empty;
    public required string KodiDomainName
    {
        get => kodiDomainName;
        set => SetProperty(ref kodiDomainName, value);
    }
    private int kodiPort;
    public required int KodiPort
    {
        get => kodiPort;
        set => SetProperty(ref kodiPort, value);
    }
    private string kodiApiUserName = string.Empty;
    public required string KodiApiUserName
    {
        get => kodiApiUserName;
        set => SetProperty(ref kodiApiUserName, value);
    }
    private string kodiApiPasword = string.Empty;
    public required string KodiApiPasword
    {
        get => kodiApiPasword;
        set => SetProperty(ref kodiApiPasword, value);
    }
    #endregion
}

