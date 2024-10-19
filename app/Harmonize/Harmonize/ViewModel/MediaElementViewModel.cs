using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Harmonize.Client.Model.Response;
using Harmonize.Model;
using Harmonize.Service;
using System.ComponentModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(MediaEntryId), nameof(MediaEntryId))]
public partial class MediaElementViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService
        ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    int currentIndex;
    public override async Task OnAppearingAsync()
    {
        await UpdateMediaElementFile();
    }
    #region OtherBindings
    private Guid mediaEntryId;
    public Guid MediaEntryId
    {
        get => mediaEntryId;
        set => SetProperty(ref mediaEntryId, value);
    }
    private double positionSliderMaximum;
    public double PositionSliderMaximum
    {
        get => positionSliderMaximum;
        set
        {
            if (positionSliderMaximum != value)
            {
                positionSliderMaximum = value;
                OnPropertyChanged(nameof(PositionSliderMaximum));
            }
        }
    }
    private double positionSliderValue;
    public double PositionSliderValue
    {
        get => positionSliderValue;
        set
        {
            if (positionSliderValue != value)
            {
                positionSliderValue = value;
                OnPropertyChanged(nameof(PositionSliderValue));
            }
        }
    }
    private Playlist? playlist;
    public Playlist? Playlist
    {
        get => playlist;
        set
        {
            if (playlist != value)
            {
                playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }
    }
    private LocalMediaEntry? mediaEntry;
    public LocalMediaEntry? MediaEntry
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
    #endregion
    #region MediaElementBindings
    private string? metadataArtist;
    public string? MetadataArtist
    {
        get => metadataArtist;
        set
        {
            if (metadataArtist != value)
            {
                metadataArtist = value;
                OnPropertyChanged(nameof(MetadataArtist));
            }
        }
    }
    private string? metadataTitle;
    public string? MetadataTitle
    {
        get => metadataTitle;
        set
        {
            if (metadataTitle != value)
            {
                metadataTitle = value;
                OnPropertyChanged(nameof(MetadataTitle));
            }
        }
    }
    private string? metadataArtworkUrl;
    public string? MetadataArtworkUrl
    {
        get => metadataArtworkUrl;
        set
        {
            if (metadataArtworkUrl != value)
            {
                metadataArtworkUrl = value;
                OnPropertyChanged(nameof(MetadataArtworkUrl));
            }
        }
    }
    private MediaSource? mediaSource;
    public MediaSource? MediaSource
    {
        get => mediaSource;
        set
        {
            if (mediaSource != value)
            {
                mediaSource = value;
                OnPropertyChanged(nameof(MediaSource));
            }
        }
    }

    #endregion
    #region MediaElementHandlers
    async Task UpdateMediaElementFile()
    {
        if (MediaEntryId == Guid.Empty)
        {
            return;
        }

        var mediaEntry = await mediaManager.GetMediaEntry(MediaEntryId);

        var mediaMetadata = await mediaManager.GetMediaMetadata();

        var xlMediaUrl = mediaManager.GetMediaMetadataArtworkUrl(mediaMetadata, "Xl");

        MetadataArtist = mediaMetadata.Artist;
        MetadataTitle = mediaMetadata.Title;
        MetadataArtworkUrl = xlMediaUrl;

        MediaSource = await mediaManager.GetMediaResource(name);

        IsPlaying = true;
        MediaEntry = mediaEntry;
    }
    public void MediaElementPropertyChanged(PropertyChangedEventArgs e, MediaElement mediaElement)
    {
        if (e.PropertyName == MediaElement.DurationProperty.PropertyName)
        {
            PositionSliderMaximum = mediaElement.Duration.TotalSeconds;
        }
    }
    public void PositionChanged(MediaPositionChangedEventArgs e)
    {
        PositionSliderValue = e.Position.TotalSeconds;
    }
    public static async Task SliderDragCompleted(object? sender, MediaElement mediaElement)
    {
        ArgumentNullException.ThrowIfNull(sender);

        if (sender is Slider slider)
        {
            var newValue = slider.Value;

            await mediaElement.SeekTo(TimeSpan.FromSeconds(newValue), CancellationToken.None);

            mediaElement.Play();
        }
    }
    public static void SliderDragStarted(MediaElement mediaElement)
    {
        mediaElement.Pause();
    }
    #endregion

    public ICommand SkipFowardCommand => new Command(async () =>
    {
        if (playlist == null)
        {
            return;
        }

        currentIndex++;

        var item = playlist.Files[currentIndex];

        await UpdateMediaElementFile(item);
    });
    public ICommand SkipBackCommand => new Command(async () =>
    {
        if (playlist == null)
        {
            return;
        }

        currentIndex--;

        var item = playlist.Files[currentIndex];

        await UpdateMediaElementFile(item);
    });
    public void PlayPauseClicked(MediaElement mediaElement)
    {
        if (IsPlaying)
        {
            mediaElement.Pause();
            IsPlaying = false;
        }
        else
        {
            mediaElement.Play();
            IsPlaying = true;
        }
    }
    public void NavigatedFrom(MediaElement mediaElement)
    {
        mediaElement.Stop();
        IsPlaying = false;
    }
}
