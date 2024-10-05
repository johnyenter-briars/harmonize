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
    FailsafeService failsafeService
) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private string? searchQuery;
    public string? SearchQuery
    {
        get => searchQuery;
        set => SetProperty(ref searchQuery, value);
    }

    private ObservableCollection<MagnetLinkSearchResult> searchResults = [];
    public ObservableCollection<MagnetLinkSearchResult> SearchResults
    {
        get => searchResults;
        set => SetProperty(ref searchResults, value);
    }

    public ICommand SearchCommand => new Command<string>(async (query) =>
    {
        if (string.IsNullOrWhiteSpace(query))
            return;

        var (results, success) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.GetPiratebaySearchResults(query);
        }, null);

        if (success)
        {
            SearchResults.Clear();
            foreach (var video in results.Value)
            {
                SearchResults.Add(video);
            }
        }
    });
    public async Task ItemTapped(MagnetLinkSearchResult magnetlinkSearchResult)
    {
        if (magnetlinkSearchResult != null)
        {
            await Shell.Current.GoToAsync(nameof(YouTubeSearchResultEditPage), new Dictionary<string, object>
            {
                { nameof(YouTubeSearchResultEditViewModel.YoutubeSearchResult), magnetlinkSearchResult }
            });
        }
    }

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}

