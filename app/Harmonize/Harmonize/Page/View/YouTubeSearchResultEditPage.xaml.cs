using Harmonize.ViewModel;
using Harmonize.Service;

namespace Harmonize.Page.View;

public partial class YouTubeSearchResultEditPage : BasePage<YouTubeSearchResultEditViewModel>
{
    private readonly YouTubeSearchResultEditViewModel viewModel;

    public YouTubeSearchResultEditPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        YouTubeSearchResultEditViewModel viewModel
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
    }
}