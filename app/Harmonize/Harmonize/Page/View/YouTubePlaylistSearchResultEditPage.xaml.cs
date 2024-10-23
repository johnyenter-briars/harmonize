using Harmonize.ViewModel;
using AlohaKit.Animations;
using Harmonize.Service;
using Harmonize.Client.Model.Youtube;

namespace Harmonize.Page.View;

public partial class YouTubePlaylistSearchResultEditPage : BasePage<YouTubePlaylistSearchResultEditViewModel>
{
    private readonly YouTubePlaylistSearchResultEditViewModel viewModel;

    public YouTubePlaylistSearchResultEditPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        YouTubePlaylistSearchResultEditViewModel viewModel
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
    }
}