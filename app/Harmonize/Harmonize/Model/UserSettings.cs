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
    private bool includeTvcControlPage;
    public required bool IncludeTvcControlPage
    {
        get => includeTvcControlPage;
        set => SetProperty(ref includeTvcControlPage, value);
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

    #region TVC
    private string tvcDomainName = string.Empty;
    public required string TvcDomainName
    {
        get => tvcDomainName;
        set => SetProperty(ref tvcDomainName, value);
    }
    private int tvcPort;
    public required int TvcPort
    {
        get => tvcPort;
        set => SetProperty(ref tvcPort, value);
    }
    #endregion

    #region Harmonize Api
    private string harmonizeUserName = string.Empty;
    public required string HarmonizeUserName
    {
        get => harmonizeUserName;
        set => SetProperty(ref harmonizeUserName, value);
    }
    private string harmonizePassword = string.Empty;
    public required string HarmonizePassword
    {
        get => harmonizePassword;
        set => SetProperty(ref harmonizePassword, value);
    }
    private string urlPrefix = string.Empty;
    public required string UrlPrefix
    {
        get => urlPrefix;
        set => SetProperty(ref urlPrefix, value);
    }
    private bool useHttps;
    public required bool UseHttps
    {
        get => useHttps;
        set => SetProperty(ref useHttps, value);
    }
    #endregion
}

