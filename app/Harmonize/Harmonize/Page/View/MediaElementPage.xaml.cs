using System.ComponentModel;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize.Page.View;

public partial class MediaElementPage : BasePage<MediaElementViewModel>
{
    private readonly MediaElementViewModel viewModel;
    private readonly ILogger logger;
    public MediaElementPage(
        MediaElementViewModel viewModel,
        ILogger<MediaElementPage> logger
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.logger = logger;

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
        viewModel.PositionChanged(e);
    }

    //void OnSeekCompleted(object? sender, EventArgs e) => logger.LogInformation("Seek completed.");

    //void OnSpeedMinusClicked(object? sender, EventArgs e)
    //{
    //    if (MediaElement.Speed >= 1)
    //    {
    //        MediaElement.Speed -= 1;
    //    }
    //}

    //void OnSpeedPlusClicked(object? sender, EventArgs e)
    //{
    //    if (MediaElement.Speed < 10)
    //    {
    //        MediaElement.Speed += 1;
    //    }
    //}
    void OnPlayPauseClicked(object? sender, EventArgs e)
    {
        viewModel.PlayPauseClicked(MediaElement);
    }
    //void OnStopClicked(object? sender, EventArgs e)
    //{
    //    MediaElement.Stop();
    //}

    //void OnMuteClicked(object? sender, EventArgs e)
    //{
    //    MediaElement.ShouldMute = !MediaElement.ShouldMute;
    //}

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        viewModel.NavigatedFrom(MediaElement);
    }

    async void Slider_DragCompleted(object? sender, EventArgs e)
    {
        await MediaElementViewModel.SliderDragCompleted(sender, MediaElement);
    }

    void Slider_DragStarted(object sender, EventArgs e)
    {
        MediaElementViewModel.SliderDragStarted(MediaElement);
    }
}
