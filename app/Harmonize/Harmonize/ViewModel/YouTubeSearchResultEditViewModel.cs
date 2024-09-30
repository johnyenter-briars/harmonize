using Harmonize.Client;
using Harmonize.Client.Model.Youtube;
using Harmonize.Page.View;
using Harmonize.Service;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(YoutubeSearchResult), nameof(YoutubeSearchResult))]
public class YouTubeSearchResultEditViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        HarmonizeClient harmonizeClient,
        FailsafeService failsafeService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private YouTubeSearchResult? youtubeSearchResult;
    public YouTubeSearchResult? YoutubeSearchResult
    {
        get => youtubeSearchResult;
        set => SetProperty(ref youtubeSearchResult, value);
    }
    public ICommand DownloadCommand => new Command<YouTubeSearchResult>(async (result) =>
    {
        var mainPage = Application.Current?.MainPage;

        if (mainPage == null)
        {
            return;
        }

        var response = await harmonizeClient.DownloadYoutube(result.Id ?? "");

        await mainPage.DisplayAlert(response.SuccessMessage, response.Message, "OK");
    });

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}

