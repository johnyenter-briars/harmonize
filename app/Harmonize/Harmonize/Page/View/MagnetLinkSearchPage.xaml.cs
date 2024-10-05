using Harmonize.ViewModel;
using AlohaKit.Animations;
using Harmonize.Service;
using Harmonize.Client.Model.Youtube;
using Harmonize.Client.Model.QBT;

namespace Harmonize.Page.View;

public partial class MagnetLinkSearchPage : BasePage<MagnetLinkSearchViewModel>
{
    private readonly MagnetLinkSearchViewModel viewModel;

    public MagnetLinkSearchPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        MagnetLinkSearchViewModel viewModel
        ) : base(viewModel)
	{
		InitializeComponent();
        this.viewModel = viewModel;
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is MagnetLinkSearchResult magnetlinkSearchResult)
        {
            await viewModel.ItemTapped(magnetlinkSearchResult);
        }
    }
}
