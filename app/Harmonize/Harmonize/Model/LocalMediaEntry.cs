using Harmonize.Client.Model.Media;

namespace Harmonize.Model;

public class LocalMediaEntry : IMediaEntry
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LocalPath { get; set; }
    public MediaElementType? Type { get; set; }
    public bool? IsSynced { get; set; } = new Random().Next(0, 2) == 1;
}
