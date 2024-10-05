namespace Harmonize.Client.Model.Response;

public record Playlist
{
    public required List<string> Files { get; set; }
}
