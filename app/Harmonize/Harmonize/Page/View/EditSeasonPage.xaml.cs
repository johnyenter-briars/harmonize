using Harmonize.Client.Model.Media;
using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize.Page.View;

public partial class EditSeasonPage  : BasePage<EditSeasonViewModel>
{
    private readonly EditSeasonViewModel viewModel;
    readonly ILogger logger;
    private readonly MediaManager mediaManager;

    public EditSeasonPage(
        EditSeasonViewModel viewModel,
        ILogger<HomePage> logger,
        MediaManager mediaManager
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.logger = logger;
        this.mediaManager = mediaManager;
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is MediaEntry mediaEntry)
        {
            await viewModel.ItemTapped(mediaEntry);
        }
    }
    protected override async void OnAppearing()
    {
        await viewModel.OnAppearingAsync();
    }
}