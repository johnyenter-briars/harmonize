using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
