using Harmonize.Kodi;
using Harmonize.Page.View;
using Harmonize.Service;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class MediaControlViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    KodiClient kodiClient,
    AlertService alertService
        ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private int currSpeed = 1;
    private readonly int MAX_FASTFORWARD_SPEED = 32;
    private readonly int MAX_REWIND_SPEED = -32;

    public ICommand EscCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.InputBackAsync();
    });
    public ICommand UpCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.InputUpAsync();
    });
    public ICommand OSDCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.ShowOSD();
    });
    public ICommand LeftCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.InputLeftAsync();
    });
    public ICommand EnterCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.InputSelectAsync();
    });
    public ICommand RightCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.InputRightAsync();
    });
    public ICommand DownCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.InputDownAsync();
    });
    public ICommand TogglePlayPauseCommand => new Command<Button>(async (Button button) =>
    {
        currSpeed = 1;
        await kodiClient.TogglePlayPausePlayerAsync();
    });
    public ICommand NavigateToTvcPage => new Command<Button>(async (Button button) =>
    {
        await Shell.Current.GoToAsync($"//TvcControl");
    });
    public ICommand PowerOffCommand => new Command<Button>(async (Button button) =>
    {
        if (await alertService.ShowConfirmationAsync("Power confirmation", "Are you sure you want to power down?"))
        {
            await kodiClient.PowerOff();
        }
    });
    public ICommand OpenVideosCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.OpenVideos();
    });
    public ICommand OpenYoutubeCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.OpenYoutube();
    });
    public ICommand RewindCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.SetPlayerSpeed(DecSpeed());
    });
    public ICommand FastFowardCommand => new Command<Button>(async (Button button) =>
    {
        await kodiClient.SetPlayerSpeed(IncSpeed());
    });
    private string? searchText = "";
    public string? SearchText
    {
        get { return searchText; }
        set { SetProperty(ref searchText, value); }
    }
    public ICommand EnterTextCommand => new Command<Entry>(async (entry) =>
    {
        //Yea idk. I think there's a bug in the task scheduler engine coupled with the optimizations of the compiler
        var text = SearchText;
        await kodiClient.InputSendText(text ?? "");
        await Task.Delay(2000);
        SearchText = null;
    });
    public ICommand EnterTextChangedCommand => new Command<string>(async (string e) =>
    {
        await kodiClient.InputText(searchText ?? "");
    });
    public int IncSpeed()
    {
        if (currSpeed <= 1)
        {
            currSpeed = 2;
        }
        else if (currSpeed != MAX_FASTFORWARD_SPEED)
        {
            currSpeed *= 2;
        }

        return currSpeed;
    }
    public int DecSpeed()
    {
        if (currSpeed >= 1)
        {
            currSpeed = -2;
        }
        else if (currSpeed != MAX_REWIND_SPEED)
        {
            currSpeed *= 2;
        }

        return currSpeed;
    }

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}
