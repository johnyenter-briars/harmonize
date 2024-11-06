using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class MediaControlViewModel : BaseViewModel
{
    private int currSpeed = 1;
    private readonly int MAX_FASTFORWARD_SPEED = 32;
    private readonly int MAX_REWIND_SPEED = -32;
    public MediaControlViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        FailsafeService failsafeService)
        : base(mediaManager, preferenceManager, failsafeService)
    {
    }
    public ICommand EscCommand => new Command<Button>(async (Button button) =>
    {
        //await KodiClientSingleton.InputBackAsync();
    });
    public ICommand UpCommand => new Command<Button>(async (Button button) => 
    { 
        //await KodiClientSingleton.InputUpAsync(); 
    });
    public ICommand OSDCommand => new Command<Button>(async (Button button) => 
    { 
        //await KodiClientSingleton.ShowOSD(); 
    });
    public ICommand LeftCommand => new Command<Button>(async (Button button) => 
    { 
        //await KodiClientSingleton.InputLeftAsync(); 
    });
    public ICommand EnterCommand => new Command<Button>(async (Button button) => 
    { 
        //await KodiClientSingleton.InputSelectAsync(); 
    });
    public ICommand RightCommand => new Command<Button>(async (Button button) => 
    { 
        //await KodiClientSingleton.InputRightAsync(); 
    });
    public ICommand DownCommand => new Command<Button>(async (Button button) => 
    { 
        //await KodiClientSingleton.InputDownAsync(); 
    });
    public ICommand TogglePlayPauseCommand => new Command<Button>(async (Button button) =>
    {
        currSpeed = 1;
        //await KodiClientSingleton.TogglePlayPausePlayerAsync();
    });
    public ICommand RebootCommand => new Command<Button>(async (Button button) =>
    {
        //if (await AlertServiceSingleton.ShowConfirmationAsync("Reboot confirmation", "Are you sure you want to reboot?"))
        //{
        //    await KodiClientSingleton.PowerReboot();
        //}
    });
    public ICommand PowerOffCommand => new Command<Button>(async (Button button) =>
    {
        //if (await AlertServiceSingleton.ShowConfirmationAsync("Power confirmation", "Are you sure you want to power down?"))
        //{
        //    await KodiClientSingleton.PowerOff();
        //}
    });
    public ICommand RewindCommand => new Command<Button>(async (Button button) => 
    { 
        //await KodiClientSingleton.SetPlayerSpeed(DecSpeed()); 
    });
    public ICommand FastFowardCommand => new Command<Button>(async (Button button) => 
    { 
        //await KodiClientSingleton.SetPlayerSpeed(IncSpeed()); 
    });
    private string searchText = "";
    public string SearchText
    {
        get { return searchText; }
        set { SetProperty(ref searchText, value); }
    }
    public ICommand EnterTextCommand => new Command(async () =>
    {
        //await KodiClientSingleton.InputSendText(searchText);
        SearchText = "";
    });
    public ICommand EnterTextChangedCommand => new Command<string>(async (string e) =>
    {
        //await KodiClientSingleton.InputText(searchText);
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
