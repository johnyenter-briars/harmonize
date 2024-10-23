using Harmonize.Client;
using Harmonize.Service;
using Harmonize.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using Harmonize.Client.Model.Youtube;
using Harmonize.Page.View;

namespace Harmonize.ViewModel;

public class YouTubeSearchViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService
) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private string? searchQuery;
    public string? SearchQuery
    {
        get => searchQuery;
        set => SetProperty(ref searchQuery, value);
    }
    private bool videosChecked = true;
    public bool VideosChecked
    {
        get => videosChecked;
        set
        {
            if (videosChecked != value)
            {
                videosChecked = value;
                OnPropertyChanged();

                if (videosChecked)
                {
                    PlaylistsChecked = false;
                }
            }
        }
    }
    private bool playlistsChecked;
    public bool PlaylistsChecked
    {
        get => playlistsChecked;
        set
        {
            if (playlistsChecked != value)
            {
                playlistsChecked = value;
                OnPropertyChanged();

                if (playlistsChecked)
                {
                    VideosChecked = false;
                }
            }
        }
    }
    private bool showVideoList;
    public bool ShowVideoList
    {
        get => showVideoList && NotFetchingData;
        set
        {
            SetProperty(ref showVideoList, value);
        }
    }
    private bool showPlaylistList;
    public bool ShowPlaylistList
    {
        get => showPlaylistList && NotFetchingData;
        set
        {
            SetProperty(ref showPlaylistList, value);
        }
    }

    private ObservableCollection<YoutubeVideoSearchResult> videoSearchResults = [];
    public ObservableCollection<YoutubeVideoSearchResult> VideoSearchResults
    {
        get => videoSearchResults;
        set => SetProperty(ref videoSearchResults, value);
    }
    private ObservableCollection<YoutubePlaylistSearchResult> playlistSearchResults = [];
    public ObservableCollection<YoutubePlaylistSearchResult> PlaylistSearchResults 
    {
        get => playlistSearchResults;
        set => SetProperty(ref playlistSearchResults, value);
    }

    public ICommand SearchCommand => new Command<SearchBar>(async (searchBar) =>
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        searchBar?.Unfocus();

        ShowPlaylistList = false;
        ShowVideoList = false;

        if (VideosChecked)
        {
            var (results, success) = await FetchData(async () =>
            {
                return await failsafeService.Fallback(async () =>
                {
                    return await harmonizeClient.GetYoutubeVideoSearchResults(SearchQuery);
                }, null);
            });

            if (success)
            {
                VideoSearchResults.Clear();
                foreach (var video in results.Value)
                {
                    VideoSearchResults.Add(video);
                }

                ShowVideoList = true;
                ShowPlaylistList = false;
            }
        }
        else if (PlaylistsChecked)
        {
            var (results, success) = await FetchData(async () =>
            {
                return await failsafeService.Fallback(async () =>
                {
                    return await harmonizeClient.GetYoutubePlaylistSearchResults(SearchQuery);
                }, null);
            });

            if (success)
            {
                PlaylistSearchResults.Clear();
                foreach (var playlist in results.Value)
                {
                    PlaylistSearchResults.Add(playlist);
                }

                ShowVideoList = false;
                ShowPlaylistList = true;
            }
        }
    });
    public async Task ItemTapped(YoutubeVideoSearchResult youTubeSearchResult)
    {
        if (youTubeSearchResult != null)
        {
            await Shell.Current.GoToAsync(nameof(YouTubeSearchResultEditPage), new Dictionary<string, object>
            {
                { nameof(YouTubeSearchResultEditViewModel.YoutubeSearchResult), youTubeSearchResult }
            });
        }
    }
    public async Task ItemTapped(YoutubePlaylistSearchResult playlistSearchResult)
    {
        if (playlistSearchResult != null)
        {
            await Shell.Current.GoToAsync(nameof(YouTubePlaylistSearchResultEditPage), new Dictionary<string, object>
            {
                { nameof(YouTubePlaylistSearchResultEditViewModel.YoutubePlaylistSearchResult), playlistSearchResult }
            });
        }
    }

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}

