using System.Text.Json.Serialization;

namespace Harmonize.Client.Model.Job;

public class Job
{
    public required Guid Id { get; set; }
    public required DateTime StartedOn { get; set; }
    public required string Description { get; set; }
    public required string? ErrorMessage { get; set; }
    public required JobStatus Status { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobStatus
{
    Succeeded = 0,
    Running = 1,
    Failed = 2,
    Canceled = 3,
}
