namespace Harmonize.Client.Model.Playlist;

public record Playlist
{
    public required List<string> Files { get; set; }
}
