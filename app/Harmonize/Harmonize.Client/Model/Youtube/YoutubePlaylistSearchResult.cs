
namespace Harmonize.Client.Model.Youtube;

public class PlaylistChannel
{
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? Link { get; set; }
}

public class PlaylistThumbnail
{
    public string? Url { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}

public class YoutubePlaylistSearchResult
{
    public required string Type { get; set; }
    public required string Id { get; set; }
    public required string Title { get; set; }
    public int? VideoCount { get; set; }
    public string? Channel { get; set; }
    public List<PlaylistThumbnail>? Thumbnails { get; set; }
    public string? Link { get; set; }
}

