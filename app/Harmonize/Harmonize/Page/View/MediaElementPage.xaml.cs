using System.ComponentModel;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Harmonize.Client.Model.Response;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;
using MediaManager = Harmonize.Service.MediaManager;

namespace Harmonize.Page.View;

public partial class MediaElementPage : BasePage<MediaElementViewModel>
{
    private readonly MediaElementViewModel viewModel;
    readonly ILogger logger;
    private readonly MediaManager mediaManager;
    public MediaElementPage(
        MediaElementViewModel viewModel,
        ILogger<MediaElementPage> logger,
        MediaManager mediaManager
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.logger = logger;
        this.mediaManager = mediaManager;
        MediaElement.PropertyChanged += MediaElement_PropertyChanged;
    }
    protected override async void OnAppearing()
    {
        await viewModel.OnAppearingAsync();
    }
    void MediaElement_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        viewModel.MediaElementPropertyChanged(e, MediaElement);
    }

    //void OnMediaOpened(object? sender, EventArgs e) => logger.LogInformation("Media opened.");

    //void OnStateChanged(object? sender, MediaStateChangedEventArgs e) =>
    //    logger.LogInformation("Media State Changed. Old State: {PreviousState}, New State: {NewState}", e.PreviousState, e.NewState);

    //void OnMediaFailed(object? sender, MediaFailedEventArgs e) => logger.LogInformation("Media failed. Error: {ErrorMessage}", e.ErrorMessage);

    //void OnMediaEnded(object? sender, EventArgs e) => logger.LogInformation("Media ended.");

    void OnPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        viewModel.OnPositionChanged(e);
    }

    void OnSeekCompleted(object? sender, EventArgs e) => logger.LogInformation("Seek completed.");

    void OnSpeedMinusClicked(object? sender, EventArgs e)
    {
        if (MediaElement.Speed >= 1)
        {
            MediaElement.Speed -= 1;
        }
    }

    void OnSpeedPlusClicked(object? sender, EventArgs e)
    {
        if (MediaElement.Speed < 10)
        {
            MediaElement.Speed += 1;
        }
    }

    void OnVolumeMinusClicked(object? sender, EventArgs e)
    {
        if (MediaElement.Volume >= 0)
        {
            if (MediaElement.Volume < .1)
            {
                MediaElement.Volume = 0;

                return;
            }

            MediaElement.Volume -= .1;
        }
    }

    void OnVolumePlusClicked(object? sender, EventArgs e)
    {
        if (MediaElement.Volume < 1)
        {
            if (MediaElement.Volume > .9)
            {
                MediaElement.Volume = 1;

                return;
            }

            MediaElement.Volume += .1;
        }
    }

    void OnPlayPauseClicked(object? sender, EventArgs e)
    {
        if (viewModel.IsPlaying)
        {
            MediaElement.Pause();
            viewModel.IsPlaying = false;

        }
        else
        {
            MediaElement.Play();
            viewModel.IsPlaying = true;
        }
    }

    void OnStopClicked(object? sender, EventArgs e)
    {
        MediaElement.Stop();
    }

    void OnMuteClicked(object? sender, EventArgs e)
    {
        MediaElement.ShouldMute = !MediaElement.ShouldMute;
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        MediaElement.Stop();
        viewModel.IsPlaying = false;
        MediaElement.Handler?.DisconnectHandler();
    }

    async void Slider_DragCompleted(object? sender, EventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);

        var newValue = ((Slider)sender).Value;

        await MediaElement.SeekTo(TimeSpan.FromSeconds(newValue), CancellationToken.None);

        MediaElement.Play();
    }

    void Slider_DragStarted(object sender, EventArgs e)
    {
        MediaElement.Pause();
    }

    async void SkipForwardClicked(object sender, EventArgs e)
    {
        //currentIndex++;
        //var item = playlist.Files[currentIndex];
        //await UpdateMediaElementFile(item);
    }

    async void SkipBackClicked(object? sender, EventArgs e)
    {
        //currentIndex--;
        //var item = playlist.Files[currentIndex];
        //await UpdateMediaElementFile(item);
    }
}
