using Harmonize.Model;
using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Page.View;

public partial class VideoLibraryPage : BasePage<VideoLibraryViewModel>
{
    private Picker _optionsPicker;
    private readonly VideoLibraryViewModel viewModel;
    private readonly MediaManager mediaManager;

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
        if (!bottomMenu.IsVisible)
        {
            await bottomMenu.ShowAsync();
        }
        else
        {
            await bottomMenu.HideAsync();
        }
    }
}