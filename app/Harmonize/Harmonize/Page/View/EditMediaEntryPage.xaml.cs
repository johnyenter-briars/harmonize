using CommunityToolkit.Maui.Views;
using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Components;
using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;

namespace Harmonize.Page.View;

public partial class EditMediaEntryPage : BasePage<EditMediaEntryViewModel>
{
    private readonly EditMediaEntryViewModel viewModel;
    readonly ILogger logger;
    private readonly MediaManager mediaManager;
    private readonly HarmonizeClient harmonizeClient;
    private readonly FailsafeService failsafeService;
    private readonly AddToSeasonViewModel addToSeasonViewModel;

    public EditMediaEntryPage(
        EditMediaEntryViewModel viewModel,
        ILogger<HomePage> logger,
        MediaManager mediaManager,
        HarmonizeClient harmonizeClient,
        FailsafeService failsafeService,
        AddToSeasonViewModel addToSeasonViewModel
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.logger = logger;
        this.mediaManager = mediaManager;
        this.harmonizeClient = harmonizeClient;
        this.failsafeService = failsafeService;
        this.addToSeasonViewModel = addToSeasonViewModel;
    }
    protected override async void OnAppearing()
    {
        await viewModel.OnAppearingAsync();
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is MediaEntry mediaEntry)
        {
            await viewModel.ItemTapped(mediaEntry);
        }
    }

    private void AddToSeason_Clicked(object sender, EventArgs e)
    {
        if (viewModel.MediaEntry is null)
        {
            return;
        }

        var popup = new AddToSeasonPopup(harmonizeClient, failsafeService, addToSeasonViewModel, viewModel.MediaEntry);

        this.ShowPopup(popup);
    }
}
