using AlohaKit.Animations;
using Harmonize.Model;
using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Page.View;

public partial class VideoLibraryPage : BasePage<VideoLibraryViewModel>
{
    private Picker _optionsPicker;
    private readonly VideoLibraryViewModel viewModel;
    private readonly MediaManager mediaManager;
    private object? previousSender = null;

    public VideoLibraryPage(
        VideoLibraryViewModel viewModel,
        MediaManager mediaManager
        ) : base(viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.mediaManager = mediaManager;
    }
    protected override async void OnAppearing()
    {
        await viewModel.OnAppearingAsync();
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is LocalMediaEntry mediaEntry)
        {
            //await viewModel.ItemTapped(mediaEntry);
        }
    }
    private async void OnOpenBottomSheetClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
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
        }

        previousSender = sender;
    }
}