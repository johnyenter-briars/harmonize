using Harmonize.Client;
using Harmonize.Client.Model.Job;
using Harmonize.Extensions;
using Harmonize.Page.View;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class JobListViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService,
    ILogger<JobListViewModel> logger
        ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private readonly HarmonizeClient harmonizeClient = harmonizeClient;
    #region Bindings
    private ObservableCollection<Job> jobs = [];
    public ObservableCollection<Job> Jobs
    {
        get { return jobs; }
        set { SetProperty(ref jobs, value); }
    }
    private bool orderByName;
    public bool OrderByName
    {
        get => orderByName;
        set
        {
            if (orderByName != value)
            {
                orderByName = value;
                OnPropertyChanged();

                if (orderByName)
                {
                    OrderByStatus = false;
                }
            }
        }
    }
    private bool orderByStatus;
    public bool OrderByStatus
    {
        get => orderByStatus;
        set
        {
            if (orderByStatus != value)
            {
                orderByStatus = value;
                OnPropertyChanged();

                if (orderByStatus)
                {
                    OrderByName = false;
                }
            }
        }
    }
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
    #endregion

    public ICommand ItemTappedCommand => new Command<Job>(async entry =>
    {
        await ItemTapped(entry);
    });
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public ICommand LoadMoreCommand => new Command(async () => await LoadMore());
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

        var (response, success) = await failsafeService.Fallback(async () =>
            await harmonizeClient.GetJobsPaging(Limit, skip, SearchQuery), null);

        if (response?.Value is not { Count: > 0 })
        {
            OutOfRecords = true;
            return;
        }

        foreach (var m in response?.Value ?? [])
        {
            Jobs.Add(m);
        }
    }
    async Task Refresh()
    {
        skip = 0;
        OutOfRecords = false;

        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () =>
                await harmonizeClient.GetJobsPaging(Limit, skip, SearchQuery), null);
        });

        Jobs.Clear();
        foreach (var m in response?.Value ?? [])
        {
            Jobs.Add(m);
        }
    }
    public async Task ItemTapped(Job job)
    {
        if (job != null)
        {
            await Shell.Current.GoToAsync(nameof(EditJobPage), false, new Dictionary<string, object>
            {
                { nameof(EditJobViewModel.JobId), job.Id }
            });
        }
    }
    public override async Task OnAppearingAsync()
    {
        await Task.CompletedTask;
    }
}
