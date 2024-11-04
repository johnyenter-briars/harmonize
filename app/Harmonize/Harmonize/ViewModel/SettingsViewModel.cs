using Harmonize.Client;
using Harmonize.Model;
using Harmonize.Service;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class SettingsViewModel : BaseViewModel
{
    public SettingsViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        FailsafeService failsafeService,
        HarmonizeClient harmonizeClient
    ) : base(mediaManager, preferenceManager, failsafeService)
    {
        UserSettings = preferenceManager.UserSettings;

        UserSettings.PropertyChanged += UserSettings_PropertyChanged;
        this.harmonizeClient = harmonizeClient;
    }

    private void UserSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        preferenceManager.SetUserSetttings(UserSettings);

        harmonizeClient
            .SetPort(UserSettings.Port)
            .SetHostName(UserSettings.DomainName);
    }
    private UserSettings? userSettings;
    public UserSettings UserSettings
    {
        get
        {
            userSettings ??= preferenceManager.UserSettings;

            return userSettings;
        }
        set
        {
            SetProperty(ref userSettings, value);
        }
    }

    private string buildVersion = Assembly.GetExecutingAssembly().GetName().ToString();
    public string BuildVersion { get => buildVersion; set => SetProperty(ref buildVersion, value); }
    private Command? saveChangesCommand;
    private readonly HarmonizeClient harmonizeClient;

    public ICommand SaveChangesCommand
    {
        get
        {
            saveChangesCommand ??= new Command(() =>
            {
                preferenceManager
                    .SetUserSetttings(UserSettings);

                harmonizeClient
                    .SetPort(UserSettings.Port)
                    .SetHostName(UserSettings.DomainName);
            });

            return saveChangesCommand;
        }
    }

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}
