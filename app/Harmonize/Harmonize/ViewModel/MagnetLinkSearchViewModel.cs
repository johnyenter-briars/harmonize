using Harmonize.Client;
using Harmonize.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Harmonize.Page.View;
using Harmonize.Client.Model.QBT;

namespace Harmonize.ViewModel;

public class MagnetLinkSearchViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService,
    AlertService alertService
) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    #region Bindings
    private string? searchQuery;
    public string? SearchQuery
    {
        get => searchQuery;
        set => SetProperty(ref searchQuery, value);
    }
    private bool piratebayChecked = true;
    private bool xt1337Checked;
    public bool PiratebayChecked
    {
        get => piratebayChecked;
        set
        {
            if (piratebayChecked != value)
            {
                piratebayChecked = value;
                OnPropertyChanged();

                if (piratebayChecked)
                {
                    Xt1337Checked = false;
                }
            }
        }
    }
    public bool Xt1337Checked
    {
        get => xt1337Checked;
        set
        {
            if (xt1337Checked != value)
            {
                xt1337Checked = value;
                OnPropertyChanged();

                if (xt1337Checked)
                {
                    PiratebayChecked = false;
                }
            }
        }
    }
    private ObservableCollection<MagnetLinkSearchResult> searchResults = [];
    public ObservableCollection<MagnetLinkSearchResult> SearchResults
    {
        get => searchResults;
        set => SetProperty(ref searchResults, value);
    }
    private bool fetchingData = false;
    public bool FetchingData
    {
        get { return fetchingData; }
        set { SetProperty(ref fetchingData, value); }
    }
    private bool notFetchingData = true;
    public bool NotFetchingData
    {
        get { return notFetchingData; }
        set { SetProperty(ref notFetchingData, value); }
    }
    private MagnetLinkSearchResult selectedSearchResult;
    public MagnetLinkSearchResult SelectedSearchResult
    {
        get { return selectedSearchResult; }
        set { SetProperty(ref selectedSearchResult, value); }
    }
    #endregion

    public ICommand SearchCommand => new Command<SearchBar>(async (searchBar) =>
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        searchBar?.Unfocus();

        var query = SearchQuery;

        var (results, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () =>
            {
                if (PiratebayChecked)
                {
                    return await harmonizeClient.GetPiratebaySearchResults(query);
                }
                else if (Xt1337Checked)
                {
                    return await harmonizeClient.GetXT1337SearchResults(query);
                }
                else return null;
            }, null);
        });

        if (success)
        {
            SearchResults.Clear();
            foreach (var video in results?.Value ?? [])
            {
                SearchResults.Add(video);
            }
        }
    });
    public async Task ItemTapped(MagnetLinkSearchResult magnetlinkSearchResult)
    {
        bool startDownload = await alertService.ShowConfirmationAsync("Confirm", "Are you sure you want to start this download?", "Yes", "No");

        if (startDownload)
        {
            var (results, success) = await failsafeService.Fallback(async () =>
            {
                return await harmonizeClient.AddTorrentResponse(new AddTorrentsRequest
                {
                    MagnetLinks = [magnetlinkSearchResult.MagnetLink]
                });
            }, null);

            if (success)
            {
                await alertService.ShowAlertAsync("Success!", "Torrent added");
            }
        }
    }
    async Task<T> FetchData<T>(Func<Task<T>> callback)
    {
        FetchingData = true;
        NotFetchingData = false;

        T response = await callback();

        FetchingData = false;
        NotFetchingData = true;

        return response;
    }

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}

