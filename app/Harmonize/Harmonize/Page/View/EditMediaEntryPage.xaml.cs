using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize.Page.View;

public partial class EditMediaEntryPage : BasePage<EditMediaEntryViewModel>
{
    private readonly EditMediaEntryViewModel viewModel;
    readonly ILogger logger;
    private readonly MediaManager mediaManager;

    public EditMediaEntryPage(
        EditMediaEntryViewModel viewModel,
        ILogger<HomePage> logger,
        MediaManager mediaManager
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.logger = logger;
        this.mediaManager = mediaManager;
    }
    protected override async void OnAppearing()
    {
        await viewModel.OnAppearingAsync();
    }
}