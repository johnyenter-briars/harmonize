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
    public MediaElementViewModel()
    {


    }
}
