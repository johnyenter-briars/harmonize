using AlohaKit.Animations;
using CommunityToolkit.Maui.Views;
using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Components;
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
    private readonly HarmonizeClient harmonizeClient;
    private readonly FailsafeService failsafeService;
    private readonly AddToSeasonViewModel addToSeasonViewModel;
    private object? previousSender = null;

    public VideoLibraryPage(
        VideoLibraryViewModel viewModel,
        MediaManager mediaManager,
        ILogger<VideoLibraryPage> logger,
        HarmonizeClient harmonizeClient,
        FailsafeService failsafeService,
        AddToSeasonViewModel addToSeasonViewModel
        ) : base(viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.mediaManager = mediaManager;
        this.logger = logger;
        this.harmonizeClient = harmonizeClient;
        this.failsafeService = failsafeService;
        this.addToSeasonViewModel = addToSeasonViewModel;
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
            if (button.Text != "Send to Media System" && 
                button.Text != "Add to Season" && 
                button.Text != "✕")
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

                if (button.Text == "Add to Season")
                {
                    await bottomMenu.HideAsync();
                }

                if (button.Text == "✕")
                {
                    await bottomMenu.HideAsync();
                }
            }

            previousSender = sender;

            if (button.Text == "Add to Season")
            {
                var mediaEntry = viewModel.SelectedMediaEntry;

                var popup = new AddToSeasonPopup(harmonizeClient, failsafeService, addToSeasonViewModel, mediaEntry);

                this.ShowPopup(popup);
            }
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
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
    private async void OnFilterClicked(object sender, EventArgs e)
    {
        if(!filterMenu.IsVisible)
        {
            await filterMenu.ShowAsync();
        }
        else
        {
            await filterMenu.HideAsync();
        }
    }
}