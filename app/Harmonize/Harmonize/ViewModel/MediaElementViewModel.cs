using Harmonize.Service;

namespace Harmonize.ViewModel;

public partial class MediaElementViewModel : BaseViewModel
{

    private bool isPlaying;
    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            if (isPlaying != value)
            {
                isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
    }
    public MediaElementViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager
        ) : base(mediaManager, preferenceManager)
    {
    }
}
