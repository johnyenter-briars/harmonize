using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.ViewModel;

public class ErrorPopupViewModel : BaseViewModel
{
    public ErrorPopupViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager
        ) : base(mediaManager, preferenceManager)
    {
    }

    public override Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }
}
