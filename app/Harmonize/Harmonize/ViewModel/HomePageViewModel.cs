using Harmonize.Service;

namespace Harmonize.ViewModel;

public class HomePageViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}
