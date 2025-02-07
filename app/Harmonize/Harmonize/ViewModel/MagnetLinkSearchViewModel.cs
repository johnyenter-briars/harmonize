using Harmonize.Client;
using Harmonize.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Harmonize.Client.Model.QBT;
using Microsoft.Extensions.Logging;
using Harmonize.Client.Model.Media;

namespace Harmonize.ViewModel;

public class MagnetLinkSearchViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService,
    AlertService alertService,
    ILogger<MagnetLinkSearchViewModel> logger
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
    #endregion

    public ICommand SearchCommand => new Command<SearchBar>(async (searchBar) =>
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        searchBar?.Unfocus();

        var (results, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () =>
            {
                if (PiratebayChecked)
                {
                    return await harmonizeClient.GetPiratebaySearchResults(SearchQuery);
                }
                else if (Xt1337Checked)
                {
                    return await harmonizeClient.GetXT1337SearchResults(SearchQuery);
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
        var choice = await Application.Current.MainPage.DisplayActionSheet(
            "Type of Entry", "Cancel", null, 
            ["Movie", "Season", "Episode", "Song", "Audiobook"]);

        if (choice == "Cancel") return;

        var request = choice switch
        {
            "Movie" => new AddQbtDownloadsRequest
                {
                    MagnetLinks = [magnetlinkSearchResult.MagnetLink],
                    Name = magnetlinkSearchResult.Name,
                    Type = MediaEntryType.Video,
                    VideoType = VideoType.Movie,
                    AudioType = null,
                    CreateSeason = null,
                },
            "Season" => new AddQbtDownloadsRequest
                {
                    MagnetLinks = [magnetlinkSearchResult.MagnetLink],
                    Name = magnetlinkSearchResult.Name,
                    Type = MediaEntryType.Video,
                    VideoType = VideoType.Episode,
                    CreateSeason = true,
                    AudioType = null,
                },
            "Episode" => new AddQbtDownloadsRequest
                {
                    MagnetLinks = [magnetlinkSearchResult.MagnetLink],
                    Name = magnetlinkSearchResult.Name,
                    Type = MediaEntryType.Video,
                    VideoType = VideoType.Episode,
                    CreateSeason = false,
                    AudioType = null,
                },
            "Song" => null,
            "Audiobook" => null,
            _ => null,
        };

        if (request is null)
        {
            logger.LogInformation($"Choice: {choice} not yet supported.");
            return;
        }

        var (results, success) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.AddQbtDownload(request);
        }, null);

        if (success)
        {
            await alertService.ShowAlertSnackbarAsync("Download started!");
        }
        else
        {
            await alertService.ShowAlertSnackbarAsync("Failure to start download.");
        }
    }
    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}

