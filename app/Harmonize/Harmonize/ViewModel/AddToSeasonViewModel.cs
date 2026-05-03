using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Season;
using Harmonize.Page.View;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class AddToSeasonViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    ILogger<VideoLibraryPage> logger,
    HarmonizeClient harmonizeClient,
    AlertService alertService,
    RecentSeasonManager recentSeasonManager
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private MediaEntry? mediaEntry;
    public MediaEntry? MediaEntry
    {
        get => mediaEntry;
        set => SetProperty(ref mediaEntry, value);
    }
    private string? searchQuery;
    public string? SearchQuery
    {
        get => searchQuery;
        set => SetProperty(ref searchQuery, value);
    }
    public ICommand SearchCommand => new Command<SearchBar>(async (searchBar) =>
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        searchBar?.Unfocus();

        await Refresh();
    });
    private ObservableCollection<Season> seasons = [];
    public ObservableCollection<Season> Seasons
    {
        get { return seasons; }
        set { SetProperty(ref seasons, value); }
    }
    private ObservableCollection<Season> recentSeasons = [];
    public ObservableCollection<Season> RecentSeasons
    {
        get { return recentSeasons; }
        set => SetProperty(ref recentSeasons, value);
    }
    private bool recentSeasonsVisible = false;
    public bool RecentSeasonsVisible
    {
        get { return recentSeasonsVisible; }
        set => SetProperty(ref recentSeasonsVisible, value);
    }
    private bool outOfRecords = false;
    public bool OutOfRecords
    {
        get { return outOfRecords; }
        set { SetProperty(ref outOfRecords, value); }
    }
    const int Limit = 10;
    int skip = 0;

    public void Initialize(MediaEntry mediaEntry)
    {
        MediaEntry = mediaEntry;
        SearchQuery = null;
        Seasons.Clear();
        LoadRecentSeasons();
    }

    async Task Refresh()
    {
        skip = 0;
        OutOfRecords = false;

        if (SearchQuery is null) return;

        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetSeasonsPaging(SearchQuery, Limit, skip), null);
        });

        Seasons.Clear();
        foreach (var m in response?.Value ?? [])
        {
            Seasons.Add(m);
        }
    }
    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }

    void LoadRecentSeasons()
    {
        var recent = recentSeasonManager.GetRecentSeasons();

        RecentSeasons.Clear();

        foreach (var season in recent)
        {
            RecentSeasons.Add(season);
        }

        RecentSeasonsVisible = RecentSeasons.Count > 0;
    }

    void SaveRecentSeason(Season season)
    {
        recentSeasonManager.SaveRecentSeason(season);
        var recent = recentSeasonManager.GetRecentSeasons();

        RecentSeasons.Clear();

        foreach (var item in recent)
        {
            RecentSeasons.Add(item);
        }

        RecentSeasonsVisible = RecentSeasons.Count > 0;
    }

    internal async Task ItemTapped(Season season)
    {
        var request = new AssociateToSeasonRequest
        {
            SeasonId = season.Id,
            MediaEntryIds = [MediaEntry!.Id],
        };

        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () =>
                await harmonizeClient.AssociateToSeason(request), null);
        });

        if (success)
        {
            if (MediaEntry is not null)
            {
                MediaEntry.SeasonId = season.Id;
            }

            SaveRecentSeason(season);
            await alertService.ShowAlertSnackbarAsync("Added to season");
        }
    }
}
