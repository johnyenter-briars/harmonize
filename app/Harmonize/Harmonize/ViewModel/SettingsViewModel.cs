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
    private string domainName = PreferenceManager.GetDomainName();
    public string DomainName { get => domainName; set => SetProperty(ref domainName, value); }

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
    internal SettingsViewModel()
    {
    }

    private void SaveChanges()
    {
        PreferenceManager.SetPreferences(domainName);
    }
}
