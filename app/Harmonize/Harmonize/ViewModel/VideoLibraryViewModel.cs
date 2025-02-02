using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Transfer;
using Harmonize.Extensions;
using Harmonize.Page.View;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class VideoLibraryViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    ILogger<VideoLibraryPage> logger,
    HarmonizeClient harmonizeClient,
    AlertService alertService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public ICommand LoadMoreCommand => new Command(async () => await LoadMore());
    public ICommand MoreInfoCommand => new Command<MediaEntry>(entry =>
    {
    });
    public ICommand ItemTappedCommand => new Command<MediaEntry>(async entry =>
    {
        await ItemTapped(entry);
    });
    public ICommand SendToMediaSystemCommand => new Command(async () =>
    {
        var (jobResponse, success) = await failsafeService.Fallback(
            async () => await harmonizeClient.StartTransfer(TransferDestination.MediaSystem, SelectedMediaEntry), null);

        if (success)
        {
            await alertService.ShowConfirmationAsync("Success", "Job created successfully.", "Ok");
        }
    });
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
    private MediaEntry selectedMediaEntry;
    public MediaEntry SelectedMediaEntry
    {
        get { return selectedMediaEntry; }
        set { SetProperty(ref selectedMediaEntry, value); }
    }
    public ICommand OpenBottomSheetCommand => new Command<MediaEntry>(entry =>
    {
        SelectedMediaEntry = entry;
    });
    private List<string> options = ["foo", "bar"];
    public List<string> Options
    {
        get { return options; }
        set { SetProperty(ref options, value); }
    }
    private ObservableCollection<MediaEntry> mediaEntries = [];
    public ObservableCollection<MediaEntry> MediaEntries
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }
    const int Limit = 10;
    int skip = 0;

    async Task LoadMore()
    {
        skip += Limit;

        var (response, success) = SearchQuery is null ?
            await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetVideoPaging(Limit, skip), null);
        })
            :
            await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetVideoPaging(SearchQuery, Limit, skip), null);
        });

        foreach (var m in response?.Value ?? [])
        {
            MediaEntries.Add(m);
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

        var (response, success) = SearchQuery is null ?
            await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetVideoPaging(Limit, skip), null);
        })
            :
            await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetVideoPaging(SearchQuery, Limit, skip), null);
        });


        MediaEntries.Clear();
        foreach (var m in response?.Value ?? [])
        {
            MediaEntries.Add(m);
        }
    }
    public async Task ItemTapped(MediaEntry mediaEntry)
    {
        await Shell.Current.GoToAsync(nameof(EditMediaEntryPage), new Dictionary<string, object>
        {
            { nameof(EditMediaEntryViewModel.MediaEntryId), mediaEntry.Id },
            { nameof(EditMediaEntryViewModel.MediaEntry), mediaEntry }
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
