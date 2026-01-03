using Harmonize.Kodi;
using Harmonize.Page.View;
using Harmonize.Service;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class TvcControlViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    KodiClient kodiClient,
    AlertService alertService
        ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand NavigateToMediaControl => new Command<Button>(async (Button button) =>
    {
        await Shell.Current.GoToAsync($"//MediaControl", false);
    });
    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}
