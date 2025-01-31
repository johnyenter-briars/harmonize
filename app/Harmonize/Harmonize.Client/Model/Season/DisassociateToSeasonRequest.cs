namespace Harmonize.Client.Model.Season;

public class DisassociateToSeasonRequest
{
    public required Guid SeasonId { get; set; }
    public required List<Guid> MediaEntryIds { get; set; }
}
