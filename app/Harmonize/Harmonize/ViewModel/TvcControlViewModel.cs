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
    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}
