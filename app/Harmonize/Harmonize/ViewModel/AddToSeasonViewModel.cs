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
    HarmonizeClient harmonizeClient
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
    private bool outOfRecords = false;
    public bool OutOfRecords
    {
        get { return outOfRecords; }
        set { SetProperty(ref outOfRecords, value); }
    }
    const int Limit = 10;
    int skip = 0;
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

    internal async Task ItemTapped(Season season)
    {
        //TODO
    }
}
