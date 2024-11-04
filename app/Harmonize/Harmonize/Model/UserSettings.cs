namespace Harmonize.Model;

public class UserSettings : NotifyPropertyChangedBase
{
    private string domainName = string.Empty;
    public string DomainName
    {
        get => domainName;
        set => SetProperty(ref domainName, value);
    }

    private int port;
    public int Port
    {
        get => port;
        set => SetProperty(ref port, value);
    }

    private string defaultPageOnLaunch = string.Empty;
    public string DefaultPageOnLaunch
    {
        get => defaultPageOnLaunch;
        set => SetProperty(ref defaultPageOnLaunch, value);
    }

    private bool resetDatabaseOnLaunch;
    public bool ResetDatabaseOnLaunch
    {
        get => resetDatabaseOnLaunch;
        set => SetProperty(ref resetDatabaseOnLaunch, value);
    }

    private bool includeMediaControlPage;
    public bool IncludeMediaControlPage
    {
        get => includeMediaControlPage;
        set => SetProperty(ref includeMediaControlPage, value);
    }
}

