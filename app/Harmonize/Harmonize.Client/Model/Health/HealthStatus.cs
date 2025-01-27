using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Client.Model.Health;

public class Uptime
{
    public required int Seconds { get; set; }
    public required int Hours { get; set; }
    public required int Minutes { get; set; }
}

public class Drive
{
    public required string Path { get; set; }
    public required double SpaceUsed { get; set; }
}

public class HealthStatus
{
    public required Uptime Uptime { get; set; }
    public required List<Drive> Drives { get; set; }
}

