using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize
{
    internal static class PreferenceManager
    {
        const string DomainNameKey = nameof(DomainNameKey);
        internal static void SetPreferences(string domainName)
        {
            Preferences.Default.Set(DomainNameKey, domainName);
        }
        internal static string GetDomainName()
        {
            return Preferences.Default.Get(DomainNameKey, string.Empty);
        }
    }
}
