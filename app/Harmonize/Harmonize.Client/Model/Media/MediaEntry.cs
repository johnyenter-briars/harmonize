using System.Text.Json.Serialization;

namespace Harmonize.Client.Model.Media;

public class MediaEntry : IMediaEntry
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string AbsolutePath { get; set; }
    public required MediaElementSource Source { get; set; }
    [JsonPropertyName("youtubeId")]
    public required string? YouTubeId { get; set; }
    public required string? MagnetLink { get; set; }
    public required MediaEntryType Type { get; set; }
    public required VideoType? VideoType { get; set; }
    public required AudioType? AudioType { get; set; }
    public required DateTime DateAdded { get; set; }
    public required string? CoverArtAbsolutePath { get; set; }
    [JsonPropertyName("thumbnailArtAbsolutePath")]
    public required string? ThumbNailArtAbsolutePath { get; set; }
    public required bool Transferred { get; set; }
    public required Guid? SeasonId { get; set; }
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MediaElementSource
{
    Youtube = 0,
    MagnetLink = 1,
}
public enum MediaEntryType
{
    Audio = 0,
    Video = 1,
    Subtitle = 2,
}
public enum VideoType
{
    Movie = 0,
    Episode = 1,
}
public enum AudioType
{
    Song = 0,
    AudioBook = 1,
}
