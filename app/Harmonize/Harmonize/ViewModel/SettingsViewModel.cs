using Harmonize.Model;
using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class SettingsViewModel : BaseViewModel
{
    private UserSettings? userSettings;
    public UserSettings UserSettings
    {
        get
        {
            userSettings ??= preferenceManager.UserSettings;

            return userSettings;
        }
        set => SetProperty(ref userSettings, value);
    }

    private string buildVersion = Assembly.GetExecutingAssembly().GetName().ToString();
    public string BuildVersion { get => buildVersion; set => SetProperty(ref buildVersion, value); }
    private Command? saveChangesCommand;
    public ICommand SaveChangesCommand
    {
        get
        {
            saveChangesCommand ??= new Command(() =>
            {
                preferenceManager
                    .SetUserSetttings(UserSettings);
            });

            return saveChangesCommand;
        }
    }
    public SettingsViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager
        ) : base(mediaManager, preferenceManager)
    {
    }
}
