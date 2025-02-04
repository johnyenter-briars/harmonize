using CommunityToolkit.Maui.Views;
using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Season;
using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Components;

public partial class AddToSeasonPopup : Popup
{
    private readonly HarmonizeClient harmonizeClient;
    private readonly FailsafeService failsafeService;
    private readonly AddToSeasonViewModel viewModel;
    private readonly MediaEntry mediaEntry;

    public AddToSeasonPopup(
        HarmonizeClient harmonizeClient,
        FailsafeService failsafeService,
        AddToSeasonViewModel viewModel,
        MediaEntry mediaEntry
        )
    {
        InitializeComponent();
        this.harmonizeClient = harmonizeClient;
        this.failsafeService = failsafeService;
        this.viewModel = viewModel;
        this.mediaEntry = mediaEntry;
        this.BindingContext = viewModel;

        Task.Run(() =>
        {
            Task.Delay(200).ContinueWith(_ => MainThread.BeginInvokeOnMainThread(() =>
            {
                this.viewModel.MediaEntry = mediaEntry;

                searchBar?.Focus();
            }));
        });
    }
    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        viewModel.SearchQuery = null;
        viewModel.Seasons.Clear();

        await CloseAsync();
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Season season)
        {
            await viewModel.ItemTapped(season);
        }
    }
    public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

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
            selectedViewCell.View.BackgroundColor = null;
        }

        listView.SelectedItem = null;
    }
}