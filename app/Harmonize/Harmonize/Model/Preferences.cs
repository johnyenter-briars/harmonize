using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Model;

public class UserSettings
{
    public required string DomainName { get; set; }
    public required int Port { get; set; }
    public required string DefaultPageOnLaunch { get; set; }
    public required bool ResetDatabaseOnLaunch { get; set; }
}
