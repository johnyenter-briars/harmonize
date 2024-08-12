using Harmonize.Client;
using Harmonize.Client.Model.Youtube;
using Harmonize.Page.View;
using Harmonize.Service;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(YoutubeSearchResult), nameof(YoutubeSearchResult))]
public class YouTubeSearchResultEditViewModel : BaseViewModel
{
    public YouTubeSearchResultEditViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        HarmonizeClient harmonizeClient
        )
        : base(mediaManager, preferenceManager)
    {
        this.harmonizeClient = harmonizeClient;
    }
    private YouTubeSearchResult? youtubeSearchResult;
    public YouTubeSearchResult? YoutubeSearchResult
    {
        get => youtubeSearchResult;
        set => SetProperty(ref youtubeSearchResult, value);
    }

    private ICommand downloadCommand;
    private readonly HarmonizeClient harmonizeClient;

    public ICommand DownloadCommand => downloadCommand ??= new Command<YouTubeSearchResult>(async (result) =>
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

