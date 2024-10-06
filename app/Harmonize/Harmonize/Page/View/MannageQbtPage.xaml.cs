using Harmonize.Client.Model.QBT;
using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Page.View;

public partial class ManageQbtPage : BasePage<ManageQbtViewModel>
{
    private readonly ManageQbtViewModel viewModel;

    public ManageQbtPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        ManageQbtViewModel viewModel
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ManageQbtViewModel viewModel)
        {
            await viewModel.OnAppearingAsync();
        }
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        //if (e.Item is MagnetLinkSearchResult magnetlinkSearchResult)
        //{
        //    await viewModel.ItemTapped(magnetlinkSearchResult);
        //}
    }
}