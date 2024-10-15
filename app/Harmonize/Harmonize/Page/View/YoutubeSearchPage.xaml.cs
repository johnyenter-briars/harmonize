using Harmonize.ViewModel;
using AlohaKit.Animations;
using Harmonize.Service;
using Harmonize.Client.Model.Youtube;

namespace Harmonize.Page.View;

public partial class YouTubeSearchPage : BasePage<YouTubeSearchViewModel>
{
    private readonly YouTubeSearchViewModel viewModel;

    public YouTubeSearchPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        YouTubeSearchViewModel viewModel
        ) : base(viewModel)
	{
		InitializeComponent();
        this.viewModel = viewModel;
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is YoutubeVideoSearchResult youTubeSearchResult)
        {
            await viewModel.ItemTapped(youTubeSearchResult);
        }
    }
}
