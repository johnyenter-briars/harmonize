using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Page.View;

public partial class LogPage : BasePage<LogViewModel>
{
    private readonly LogViewModel viewModel;
    private readonly MediaManager mediaManager;

    public LogPage(
        LogViewModel viewModel,
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
}