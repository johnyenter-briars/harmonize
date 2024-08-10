using Harmonize.ViewModel;
using AlohaKit.Animations;
using Harmonize.Service;

namespace Harmonize.Page.View;

public partial class YouTubeSearchPage : BasePage<YouTubeSearchViewModel>
{
	public YouTubeSearchPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        YouTubeSearchViewModel viewModel
        ) : base(viewModel)
	{
		InitializeComponent();
	}
}
