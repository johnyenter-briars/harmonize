using Harmonize.Client.Model.Media;

namespace Harmonize.Client.Model.QBT;

public class AddQbtDownloadsRequest
{
    public required List<string> MagnetLinks { get; set; }
    public required MediaEntryType Type { get; set; }
    public required VideoType? VideoType { get; set; }
    public required bool? CreateSeason { get; set; }
    public required AudioType? AudioType { get; set; }
}
