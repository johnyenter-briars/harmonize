using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize.Page.View;
public partial class EditJobPage : BasePage<EditJobViewModel>
{
    private readonly EditJobViewModel viewModel;
    readonly ILogger logger;
    private readonly MediaManager mediaManager;

    public EditJobPage(
        EditJobViewModel viewModel,
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
        base.OnAppearing();

        if (BindingContext is EditJobViewModel viewModel)
        {
            await viewModel.RetrieveJob();
        }
    }
}
