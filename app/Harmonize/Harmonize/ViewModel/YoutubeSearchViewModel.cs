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
    HarmonizeClient harmonizeClient
) : BaseViewModel(mediaManager, preferenceManager)
{
    private string? searchQuery;
    public string? SearchQuery
    {
        get => searchQuery;
        set => SetProperty(ref searchQuery, value);
    }

    private ObservableCollection<YouTubeSearchResult> searchResults = [];
    public ObservableCollection<YouTubeSearchResult> SearchResults
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
    public async Task ItemTapped(YouTubeSearchResult youTubeSearchResult)
    {
        if (youTubeSearchResult != null)
        {
            await Shell.Current.GoToAsync(nameof(YouTubeSearchResultEditPage), new Dictionary<string, object>
            {
                { nameof(YouTubeSearchResultEditViewModel.YoutubeSearchResult), youTubeSearchResult }
            });
        }
    }

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}

