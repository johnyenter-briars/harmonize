﻿using Harmonize.Client;
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
            await alertService.ShowAlertSnackbarAsync("Job created successfully.");
        }
    });
    #region Filters
    private bool filterByMovie = true;
    public bool FilterByMovie
    {
        get => filterByMovie;
        set
        {
            SetProperty(ref filterByMovie, value);
            if (!firstLoad)
            {
                FetchingData = true;
            }
        }
    }
    private bool filterByEpisode = true;
    public bool FilterByEpisode
    {
        get => filterByEpisode;
        set
        {
            SetProperty(ref filterByEpisode, value);
            if (!firstLoad)
            {
                FetchingData = true;
            }
        }
    }
    private bool filterByTransferred = false;
    public bool FilterByTransferred
    {
        get => filterByTransferred;
        set
        {
            SetProperty(ref filterByTransferred, value);
            if (!firstLoad)
            {
                FetchingData = true;
            }
        }
    }
    #endregion
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
    bool firstLoad = true;
    int skip = 0;
    async Task LoadMore()
    {
        if (OutOfRecords) return;

        if (firstLoad)
        {
            firstLoad = false;
        }
        else
        {
            skip += Limit;
        }

        var types = new List<VideoType>();

        if (FilterByEpisode)
        {
            types.Add(VideoType.Episode);
        }

        if (FilterByMovie)
        {
            types.Add(VideoType.Movie);
        }

        var (response, success) = await failsafeService.Fallback(async () =>
            await harmonizeClient.GetVideosPaging(Limit, skip, SearchQuery, types, FilterByTransferred), null);

        if (response?.Value is not { Count: > 0 })
        {
            OutOfRecords = true;
            return;
        }

        foreach (var m in response?.Value ?? [])
        {
            MediaEntries.Add(m);
        }
    }
    public ICommand OpenSearchCommand => new Command<SearchBar>((searchBar) =>
    {
        if (SearchBarVisible)
        {
            SearchQuery = null;
        }

        SearchBarVisible = !SearchBarVisible;

        if (SearchBarVisible)
        {
            searchBar?.Focus();
        }
        else
        {
            searchBar?.Unfocus();
        }
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

        var types = new List<VideoType>();

        if (FilterByEpisode)
        {
            types.Add(VideoType.Episode);
        }

        if (FilterByMovie)
        {
            types.Add(VideoType.Movie);
        }

        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () =>
                await harmonizeClient.GetVideosPaging(Limit, skip, SearchQuery, types, FilterByTransferred), null);
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
        await Task.CompletedTask;
    }
}
