using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.ViewModel;

public class HomePageViewModel : BaseViewModel
{
    public HomePageViewModel(
        MediaManager mediaManager, 
        PreferenceManager preferenceManager) : base(mediaManager, preferenceManager)
    {
    }
}
