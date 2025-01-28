using Harmonize.Client.Model.Transfer;
using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Page.View;

public partial class TransferListPage : BasePage<TransferListViewModel>
{
    private readonly TransferListViewModel viewModel;
    private readonly MediaManager mediaManager;

    public TransferListPage(
        TransferListViewModel viewModel,
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
        if (e.Item is TransferProgress job)
        {
            await viewModel.ItemTapped(job);
        }
    }
}