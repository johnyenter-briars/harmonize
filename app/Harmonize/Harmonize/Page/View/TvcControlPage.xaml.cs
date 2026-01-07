using AlohaKit.Animations;
using Harmonize.Kodi;
using Harmonize.Service;
using Harmonize.TVC;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize.Page.View;

public partial class TvcControlPage : BasePage<TvcControlViewModel>
{
    private readonly TvcControlViewModel viewModel;
    readonly ILogger logger;
    private readonly MediaManager mediaManager;
    private readonly KodiClient kodiClient;
    private readonly TvcClient tvcClient;

    public TvcControlPage(
        TvcControlViewModel viewModel,
        ILogger<HomePage> logger,
        MediaManager mediaManager,
        KodiClient kodiClient,
        TvcClient tvcClient
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.logger = logger;
        this.mediaManager = mediaManager;
        this.kodiClient = kodiClient;
        this.tvcClient = tvcClient;
    }
    private void ScaleButton(object sender, EventArgs e)
    {
        if (sender is Microsoft.Maui.Controls.View view)
        {
            var previousScale = view.Scale;
            view.Animate(new StoryBoard(new List<AnimationBase>
              {
                 new ScaleToAnimation { Scale = 1.1, Duration = "150" },
                 new ScaleToAnimation { Scale = previousScale, Duration = "100" }
              }));
        }
    }
    private CancellationTokenSource? _volumeCts;
    private async Task StartVolumeLoop(
    Func<Task> action,
    int initialDelayMs = 0,
    int repeatDelayMs = 5)
    {
        _volumeCts?.Cancel();
        _volumeCts?.Dispose();

        _volumeCts = new CancellationTokenSource();
        var token = _volumeCts.Token;

        try
        {
            if (initialDelayMs > 0)
                await Task.Delay(initialDelayMs, token);

            await action();

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(repeatDelayMs, token);
                await action();
            }
        }
        catch (TaskCanceledException)
        {
            // Expected when released
        }
    }
    private async void VolumeUp_Pressed(object sender, EventArgs e)
    {
        await StartVolumeLoop(() => tvcClient.VolumeUp());
    }

    private async void VolumeDown_Pressed(object sender, EventArgs e)
    {
        await StartVolumeLoop(() => tvcClient.VolumeDown());
    }
    private void Volume_Released(object sender, EventArgs e)
    {
        _volumeCts?.Cancel();
        _volumeCts?.Dispose();
        _volumeCts = null;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _volumeCts?.Cancel();
        _volumeCts?.Dispose();
        _volumeCts = null;
    }
    private readonly int DoubleTapThresholdMs = 300;

    // Track last tap time per button
    private DateTime _lastVolumeUpTap = DateTime.MinValue;
    private bool _volumeUpDoubleTapDetected = false;

    private DateTime _lastVolumeDownTap = DateTime.MinValue;
    private bool _volumeDownDoubleTapDetected = false;

    private async void VolumeUp_Clicked(object sender, EventArgs e)
    {
        var now = DateTime.Now;
        if ((now - _lastVolumeUpTap).TotalMilliseconds <= DoubleTapThresholdMs)
        {
            _volumeUpDoubleTapDetected = true; // prevent single tap
            return;
        }

        _volumeUpDoubleTapDetected = false;
        _lastVolumeUpTap = now;

        await Task.Delay(DoubleTapThresholdMs);

        if (!_volumeUpDoubleTapDetected)
        {
            ScaleButton(sender, e);
            await AdjustVolumeBy(1);
        }
    }

    private async void VolumeDown_Clicked(object sender, EventArgs e)
    {
        var now = DateTime.Now;
        if ((now - _lastVolumeDownTap).TotalMilliseconds <= DoubleTapThresholdMs)
        {
            _volumeDownDoubleTapDetected = true; // prevent single tap
            return;
        }

        _volumeDownDoubleTapDetected = false;
        _lastVolumeDownTap = now;

        await Task.Delay(DoubleTapThresholdMs);

        if (!_volumeDownDoubleTapDetected)
        {
            ScaleButton(sender, e);
            await AdjustVolumeBy(-1);
        }
    }

    private async void VolumeUpBy40_Clicked(object sender, EventArgs e)
    {
        _volumeUpDoubleTapDetected = true; // block single tap
        ScaleButton(sender, e);
        await AdjustVolumeBy(40);
    }

    private async void VolumeDownBy40_Clicked(object sender, EventArgs e)
    {
        _volumeDownDoubleTapDetected = true; // block single tap
        ScaleButton(sender, e);
        await AdjustVolumeBy(-40);
    }

    private async Task AdjustVolumeBy(int steps)
    {
        if (steps == 0) return;

        Func<Task> action = steps > 0 ? tvcClient.VolumeUp : tvcClient.VolumeDown;
        int count = Math.Abs(steps);

        for (int i = 0; i < count; i++)
        {
            await action();
            await Task.Delay(10); // small delay to prevent flooding
        }
    }
    private DateTime _lastTapTimeVolUp;
    private const int DoubleTapThreshold = 300; // milliseconds

    private void VolumeUpButton_Clicked(object sender, EventArgs e)
    {
        var now = DateTime.Now;
        if ((now - _lastTapTimeVolUp).TotalMilliseconds <= DoubleTapThreshold)
        {
            // Double tap detected
            VolumeUpBy40_Clicked(sender, e);
            _lastTapTimeVolUp = DateTime.MinValue; // reset
        }
        else
        {
            // Single tap, delay execution to check for double tap
            _lastTapTimeVolUp = now;
            Task.Delay(DoubleTapThreshold).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if ((DateTime.Now - _lastTapTimeVolUp).TotalMilliseconds >= DoubleTapThreshold)
                    {
                        VolumeUp_Clicked(sender, e);
                        _lastTapTimeVolUp = DateTime.MinValue;
                    }
                });
            });
        }
    }
    private DateTime _lastTapTimeVolDown;
    private void VolumeDownButton_Clicked(object sender, EventArgs e)
    {
        var now = DateTime.Now;
        if ((now - _lastTapTimeVolDown).TotalMilliseconds <= DoubleTapThreshold)
        {
            // Double tap detected
            VolumeDownBy40_Clicked(sender, e);
            _lastTapTimeVolDown = DateTime.MinValue; // reset
        }
        else
        {
            // Single tap, delay execution to check for double tap
            _lastTapTimeVolDown = now;
            Task.Delay(DoubleTapThreshold).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if ((DateTime.Now - _lastTapTimeVolDown).TotalMilliseconds >= DoubleTapThreshold)
                    {
                        VolumeDown_Clicked(sender, e);
                        _lastTapTimeVolDown = DateTime.MinValue;
                    }
                });
            });
        }
    }
}
