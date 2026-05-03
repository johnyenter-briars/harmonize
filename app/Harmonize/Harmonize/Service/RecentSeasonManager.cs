using Harmonize.Client.Model.Season;
using Newtonsoft.Json;

namespace Harmonize.Service;

public class RecentSeasonManager
{
    private const string RecentSeasonsPreferenceKey = "AddToSeason.RecentSeasons";
    private const int RecentSeasonLimit = 10;

    public List<Season> GetRecentSeasons()
    {
        var json = Preferences.Default.Get(RecentSeasonsPreferenceKey, string.Empty);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonConvert.DeserializeObject<List<Season>>(json) ?? [];
    }

    public void SaveRecentSeason(Season season)
    {
        var recent = GetRecentSeasons()
            .Where(x => x.Id != season.Id)
            .Prepend(season)
            .Take(RecentSeasonLimit)
            .ToList();

        Preferences.Default.Set(RecentSeasonsPreferenceKey, JsonConvert.SerializeObject(recent));
    }
}
