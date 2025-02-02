using Harmonize.Client;
using Harmonize.Client.Model.Season;
using Harmonize.Extensions;
using Harmonize.Page.View;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class SeasonLibraryViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    ILogger<SeasonLibraryViewModel> logger,
    HarmonizeClient harmonizeClient,
    AlertService alertService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand ItemTappedCommand => new Command<Season>(async season =>
    {
        await ItemTapped(season);
    });
    public ICommand LoadMoreCommand => new Command(async () => await LoadMore());
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    private bool outOfRecords = false;
    public bool OutOfRecords
    {
        get { return outOfRecords; }
        set { SetProperty(ref outOfRecords, value); }
    }
    private bool searchBarVisible = false;
    public bool SearchBarVisible
    {
        get { return searchBarVisible; }
        set { SetProperty(ref searchBarVisible, value); }
    }
    private string? searchQuery;
    public string? SearchQuery
    {
        get => searchQuery;
        set => SetProperty(ref searchQuery, value);
    }
    private Season selectedSeason;
    public Season SelectedSeason
    {
        get { return selectedSeason; }
        set { SetProperty(ref selectedSeason, value); }
    }
    public ICommand OpenBottomSheetCommand => new Command<Season>(entry =>
    {
        SelectedSeason = entry;
    });
    private List<string> options = ["foo", "bar"];
    public List<string> Options
    {
        get { return options; }
        set { SetProperty(ref options, value); }
    }
    private ObservableCollection<Season> seasons = [];
    public ObservableCollection<Season> Seasons
    {
        get { return seasons; }
        set { SetProperty(ref seasons, value); }
    }
    const int Limit = 10;
    int skip = 0;
    async Task LoadMore()
    {
        if (OutOfRecords) return;

        skip += Limit;

        var (response, success) = SearchQuery is null ?
            await failsafeService.Fallback(async () => await harmonizeClient.GetSeasonsPaging(Limit, skip), null)
            :
            await failsafeService.Fallback(async () => await harmonizeClient.GetSeasonsPaging(SearchQuery, Limit, skip), null)
            ;

        if (response?.Value is not { Count: > 0 })
        {
            OutOfRecords = true;
            return;
        }

        foreach (var m in response?.Value ?? [])
        {
            Seasons.Add(m);
        }
    }
    public ICommand OpenSearchCommand => new Command(() =>
    {
        if (SearchBarVisible)
        {
            SearchQuery = null;
        }

        SearchBarVisible = !SearchBarVisible;
    });
    public ICommand SearchCommand => new Command<SearchBar>(async (searchBar) =>
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        searchBar?.Unfocus();

        await Refresh();
    });
    async Task Refresh()
    {
        skip = 0;
        OutOfRecords = false;

        var (response, success) = SearchQuery is null ?
            await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetSeasonsPaging(Limit, skip), null);
        })
            :
            await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetSeasonsPaging(SearchQuery, Limit, skip), null);
        });

        Seasons.Clear();
        foreach (var m in response?.Value ?? [])
        {
            Seasons.Add(m);
        }
    }
    public async Task ItemTapped(Season season)
    {
        await Shell.Current.GoToAsync(nameof(EditSeasonPage), new Dictionary<string, object>
        {
            { nameof(EditSeasonViewModel.Seaon), season }
        });
    }

    public override async Task OnAppearingAsync()
    {
        Task.Run(() =>
        {
            FetchingData = true;
        }).FireAndForget(ex => logger.LogError($"Error: {ex}"));

        await Task.CompletedTask;
    }
}
