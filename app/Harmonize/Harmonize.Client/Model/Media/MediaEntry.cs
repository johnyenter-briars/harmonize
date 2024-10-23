using System.Text.Json.Serialization;

namespace Harmonize.Client.Model.Media;

public class MediaEntry : IMediaEntry
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string AbsolutePath { get; set; }

    public required MediaElementSource Source { get; set; }

    public string? YouTubeId { get; set; }

    public required MediaElementType Type { get; set; }

    public required DateTime DateAdded { get; set; }
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MediaElementSource
{
    Youtube = 0,
    MagnetLink = 1,
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MediaElementType
{
    Audio = 0,
    Video = 1,
}
