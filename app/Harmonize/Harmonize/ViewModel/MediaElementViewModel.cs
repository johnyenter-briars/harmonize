using Harmonize.Model;
using Harmonize.Service;

namespace Harmonize.ViewModel;

public partial class MediaElementViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager
        ) : BaseViewModel(mediaManager, preferenceManager)
{
    private MediaEntry? mediaEntry;
    public MediaEntry? MediaEntry
    {
        get => mediaEntry;
        set
        {
            if (mediaEntry != value)
            {
                mediaEntry = value;
                OnPropertyChanged(nameof(MediaEntry));
            }
        }
    }

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
}
