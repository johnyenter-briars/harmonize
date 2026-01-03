using Harmonize.Kodi;
using Harmonize.Page.View;
using Harmonize.Service;
using Harmonize.TVC;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class TvcControlViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    KodiClient kodiClient,
    AlertService alertService,
    TvcClient tvcClient
        ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand NavigateToMediaControl => new Command<Button>(async (Button button) =>
    {
        await Shell.Current.GoToAsync($"//MediaControl", false);
    });
    public ICommand PowerOffCommand => new Command(async () =>
    {
        await tvcClient.PowerOff();
    });
    public ICommand PowerOnCommand => new Command(async () =>
    {
        await tvcClient.PowerOn();
    });
    public ICommand ToggleMute => new Command(async () =>
    {
        await tvcClient.ToggleMute();
    });
    public ICommand VolumeUp => new Command(async () =>
    {
        await tvcClient.VolumeUp();
    });
    public ICommand VolumeDown => new Command(async () =>
    {
        await tvcClient.VolumeDown();
    });
    public ICommand Hdmi1 => new Command(async () =>
    {
        await tvcClient.HdmiSwitch(HdmiInput.One);
    });
    public ICommand Hdmi2 => new Command(async () =>
    {
        await tvcClient.HdmiSwitch(HdmiInput.Two);
    });
    public ICommand Hdmi3 => new Command(async () =>
    {
        await tvcClient.HdmiSwitch(HdmiInput.Three);
    });
    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}
