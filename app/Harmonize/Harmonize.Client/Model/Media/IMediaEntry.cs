namespace Harmonize.Client.Model.Media;

public interface IMediaEntry
{
    Guid Id { get; set; }
    string Name { get; set; }
    MediaEntryType Type { get; set; }
}
