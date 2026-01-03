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

            // Repeat while held
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
}
