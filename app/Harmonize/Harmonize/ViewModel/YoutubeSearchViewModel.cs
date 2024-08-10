using Harmonize.Client;
using Harmonize.Service;
using Harmonize.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using Harmonize.Client.Model.Youtube;

namespace Harmonize.ViewModel;

public class YouTubeSearchViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient
) : BaseViewModel(mediaManager, preferenceManager)
{
    private string searchQuery;
    public string SearchQuery
    {
        get => searchQuery;
        set => SetProperty(ref searchQuery, value);
    }

    private ObservableCollection<YoutubeSearchResult> searchResults = [];
    public ObservableCollection<YoutubeSearchResult> SearchResults
    {
        get => searchResults;
        set => SetProperty(ref searchResults, value);
    }

    public ICommand SearchCommand => new Command<string>(async (query) =>
    {
        if (string.IsNullOrWhiteSpace(query))
            return;

        var results = await harmonizeClient.GetYoutubeSearchResults(query);

        SearchResults.Clear();
        foreach (var video in results.Result)
        {
            SearchResults.Add(video);
        }
    });
}

