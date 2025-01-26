using System;
using System.Text.Json.Serialization;
namespace Harmonize.Client.Model.Transfer;


public enum TransferDestination
{
    MediaSystem = 0
}

public class TransferProgress
{
    public Guid MediaEntryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransferDestination Destination { get; set; }
    public float Progress { get; set; }
    public DateTime StartTime { get; set; }
}
