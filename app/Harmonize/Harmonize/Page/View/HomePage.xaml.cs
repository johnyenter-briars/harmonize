using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize.Page.View;
public partial class HomePage : BasePage<HomePageViewModel>
{
    private readonly HomePageViewModel viewModel;
    readonly ILogger logger;
    private readonly MediaManager mediaManager;

    public HomePage(
        HomePageViewModel viewModel,
        ILogger<HomePage> logger,
        MediaManager mediaManager
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.logger = logger;
        this.mediaManager = mediaManager;
    }
}
