using System;
using System.Text.Json.Serialization;
namespace Harmonize.Client.Model.Transfer;


public enum TransferDestination
{
    MediaSystem = 0
}

public class TransferProgress
{
    public required Guid MediaEntryId { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required TransferDestination Destination { get; set; }
    public required float Progress { get; set; }
    public string ProgressFmt => $"{Progress}%";
    public required DateTime StartTime { get; set; }
}
