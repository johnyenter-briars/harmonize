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

    public ICommand SearchCommand => new Command<string>(async (query) =>
    {
        if (string.IsNullOrWhiteSpace(query))
            return;

        var (results, success) = await failsafeService.Fallback(async () =>
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

