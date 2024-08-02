using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmonize.ViewModel;

internal class SettingsViewModel : BaseViewModel
{
    private string buildVersion = Assembly.GetExecutingAssembly().GetName().ToString();
    public string BuildVersion { get => buildVersion; set => SetProperty(ref buildVersion, value); }
    private string? domainName;
    public string DomainName
    {
        get
        {
            if (domainName == null)
            {
                domainName = preferenceManager.GetDomainName();
            }

            return domainName;
        }
        set => SetProperty(ref domainName, value);
    }

    private Command? saveChangesCommand;
    public ICommand SaveChangesCommand
    {
        get
        {
            if (saveChangesCommand == null)
            {
                saveChangesCommand = new Command(SaveChanges);
            }

            return saveChangesCommand;
        }
    }
    public SettingsViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager
        ) : base(mediaManager, preferenceManager)
    {
    }

    private void SaveChanges()
    {
        preferenceManager.SetPreferences(DomainName);
    }
}
