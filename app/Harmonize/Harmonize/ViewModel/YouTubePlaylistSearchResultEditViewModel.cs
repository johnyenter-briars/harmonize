using Harmonize.Client;
using Harmonize.Client.Model.Youtube;
using Harmonize.Service;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(YoutubePlaylistSearchResult), nameof(YoutubePlaylistSearchResult))]
public class YouTubePlaylistSearchResultEditViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        HarmonizeClient harmonizeClient,
        FailsafeService failsafeService,
        AlertService alertService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private YoutubePlaylistSearchResult? playlistSearchResult;
    public YoutubePlaylistSearchResult? YoutubePlaylistSearchResult
    {
        get => playlistSearchResult;
        set => SetProperty(ref playlistSearchResult, value);
    }
    public ICommand DownloadCommand => new Command<YoutubePlaylistSearchResult>(async (result) =>
    {
        var mainPage = Application.Current?.MainPage;

        if (mainPage == null)
        {
            return;
        }

        var (response, success) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.DownloadYoutubePlaylist(result.Id);

        }, null);

        if (success && response is not null)
        {
            await alertService.ShowAlertAsync(response.SuccessMessage, response.Message);
        }
        else
        {
            await alertService.ShowAlertAsync("Failure", "Failed to download playlist");
        }
    });

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}

