using AlohaKit.Animations;
using Harmonize.Client.Model.Media;
using Harmonize.Model;
using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize.Page.View;

public partial class VideoLibraryPage : BasePage<VideoLibraryViewModel>
{
    private Picker _optionsPicker;
    private readonly VideoLibraryViewModel viewModel;
    private readonly MediaManager mediaManager;
    private readonly ILogger<VideoLibraryPage> logger;
    private object? previousSender = null;

    public VideoLibraryPage(
        VideoLibraryViewModel viewModel,
        MediaManager mediaManager,
        ILogger<VideoLibraryPage> logger
        ) : base(viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.mediaManager = mediaManager;
        this.logger = logger;
    }
    protected override async void OnAppearing()
    {
        await viewModel.OnAppearingAsync();
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is MediaEntry mediaEntry)
        {
            await viewModel.ItemTapped(mediaEntry);
        }
    }
    private async void OnOpenBottomSheetClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            if (button.Text != "Send to Media System")
            {
                var currentScale = button.Scale;
                await button.Animate(new StoryBoard(
                  [
                     new ScaleToAnimation { Scale = 2, Duration = "150" },
                 new ScaleToAnimation { Scale = currentScale, Duration = "100" }
                  ]));
            }

            if (!bottomMenu.IsVisible)
            {
                await bottomMenu.ShowAsync();
            }
            else
            {
                if (sender == previousSender)
                {
                    await bottomMenu.HideAsync();
                }

                if (button.Text == "Send to Media System")
                {
                    await bottomMenu.HideAsync();
                }
            }

            previousSender = sender;
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        //var foo = 10;
        //var grid = (Grid)sender;

        //VisualStateManager.GoToState(grid, "Tapped");
        //Task.Delay(100); // Short delay for animation effect
        //VisualStateManager.GoToState(grid, "Normal");

        Task.Run(async () =>
        {
            if (sender is Grid grid)
            {
                var shimmer = grid.FindByName<BoxView>("ShimmerEffect");
                if (shimmer != null)
                {
                    // Animate opacity for shimmer effect
                    await shimmer.FadeTo(1, 100); // Show shimmer
                    await Task.Delay(150); // Hold shimmer effect
                    await shimmer.FadeTo(0, 300); // Fade it out smoothly
                }
            }
        }).FireAndForget(ex => logger.LogError($"Error: {ex}"));
    }
    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        var foo = 10;
    }
}