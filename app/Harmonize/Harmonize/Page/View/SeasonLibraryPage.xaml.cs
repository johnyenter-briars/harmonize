using AlohaKit.Animations;
using CommunityToolkit.Maui.Views;
using Harmonize.Client;
using Harmonize.Client.Model.Season;
using Harmonize.Components;
using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Page.View;

public partial class SeasonLibraryPage : BasePage<SeasonLibraryViewModel>
{
    private Picker _optionsPicker;
    private readonly SeasonLibraryViewModel viewModel;
    private readonly MediaManager mediaManager;
    private readonly HarmonizeClient harmonizeClient;
    private readonly FailsafeService failsafeService;
    private object? previousSender = null;

    public SeasonLibraryPage(
        SeasonLibraryViewModel  viewModel,
        MediaManager mediaManager,
        HarmonizeClient harmonizeClient,
        FailsafeService failsafeService
        ) : base(viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.mediaManager = mediaManager;
        this.harmonizeClient = harmonizeClient;
        this.failsafeService = failsafeService;
    }
    protected override async void OnAppearing()
    {
        await viewModel.OnAppearingAsync();
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Season season)
        {
            await viewModel.ItemTapped(season);
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
    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        var popup = new CreateSeasonPopup(harmonizeClient, failsafeService);

        this.ShowPopup(popup);
    }
}