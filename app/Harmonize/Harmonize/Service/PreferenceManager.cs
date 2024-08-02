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
        internal void SetPreferences(string domainName)
        {
            Preferences.Default.Set(DomainNameKey, domainName);
        }
        internal string GetDomainName()
        {
            return Preferences.Default.Get(DomainNameKey, string.Empty);
        }
    }
}
