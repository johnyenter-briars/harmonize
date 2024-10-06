namespace Harmonize.Client.Model.QBT;

public class MagnetLinkSearchResult
{
    public required string MagnetLink { get; set; }
    public required int? NumberSeeders { get; set; }
    public required int? NumberLeechers { get; set; }
    public required string? Name { get; set; }
    public required int? NumberDownloads { get; set; }
    public required string? Size { get; set; }
    public required string? DatePosted { get; set; }

    public string FullItem => $"{NumberSeeders} seeders | {NumberLeechers} leechers | {Size} | {DatePosted}";
}
