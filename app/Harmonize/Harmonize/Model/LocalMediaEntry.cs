using Harmonize.Client.Model.Media;

namespace Harmonize.Model;

public class LocalMediaEntry : IMediaEntry
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LocalPath { get; set; }
}
