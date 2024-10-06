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
    void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var selectedItem = e.SelectedItem as MagnetLinkSearchResult;

        var listView = sender as ListView;

        foreach (ViewCell viewCell in listView.TemplatedItems)
        {
            if (viewCell != null)
            {
                viewCell.View.BackgroundColor = null;
            }
        }
        var selectedViewCell = listView.TemplatedItems[e.SelectedItemIndex] as ViewCell;

        if (selectedViewCell != null)
        {
            selectedViewCell.View.BackgroundColor = (Color)Application.Current.Resources["Primary"]; ;
        }

        listView.SelectedItem = null;
    }
}
