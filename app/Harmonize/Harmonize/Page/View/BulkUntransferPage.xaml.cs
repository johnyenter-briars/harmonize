using Harmonize.ViewModel;

namespace Harmonize.Page.View;

public partial class BulkUntransferPage : BasePage<BulkUntransferViewModel>
{
    private readonly BulkUntransferViewModel viewModel;

    public BulkUntransferPage(BulkUntransferViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.OnAppearingAsync();
    }
}
