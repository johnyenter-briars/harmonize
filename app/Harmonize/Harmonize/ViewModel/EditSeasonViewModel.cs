using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Season;
using Harmonize.Extensions;
using Harmonize.Page.View;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(Season), nameof(Season))]
public class EditSeasonViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    HarmonizeClient harmonizeClient,
    ILogger<EditSeasonViewModel> logger,
    AlertService alertService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private ObservableCollection<MediaEntry> mediaEntries = [];
    public ObservableCollection<MediaEntry> MediaEntries
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }
    private Season season;
    public Season Season
    {
        get => season;
        set => SetProperty(ref season, value);
    }
    public ICommand RemoveFromSeason => new Command<MediaEntry>(async (entry) =>
    {
        if (await alertService.ShowConfirmationAsync("Remove Episode", $"Are you sure you want to remove ep: {entry.Name}?"))
        {
            var request = new DisassociateToSeasonRequest
            {
                SeasonId = Season.Id,
                MediaEntryIds = [entry.Id]
            };

            var (response, success) = await failsafeService.Fallback(async () =>
                await harmonizeClient.DisassociateToSeason(request), null);

            if (success)
            {
                await alertService.ShowAlertSnackbarAsync("Removed from season");
                await Refresh();
            }
        }
    });
    public ICommand DeleteSeason => new Command(async () =>
    {
        if (await alertService.ShowConfirmationAsync("Delete Season", $"Are you sure you want to delete season: {season.Name}?") == true)
        {
            var deleteEpisodes = await alertService.ShowConfirmationAsync("Delete all episodes?", $"Do you want to delete all episodes in: {season.Name}?");

            var (response, success) = await FetchData(async () =>
            {
                return await failsafeService.Fallback(async () => await harmonizeClient.DeleteSeason(Season, deleteEpisodes), null);
            });

            if (response?.Success == true)
            {
                await Shell.Current.GoToAsync("..", false);
            }
        }
    });
    public ICommand SaveSeason => new Command(async () =>
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.UpdateSeason(season, new UpsertSeasonRequest { Name = season.Name }), null);
        });
    });
    async Task Refresh()
    {
        var (response, success) = await FetchData(async () =>
            await failsafeService.Fallback(async () => await harmonizeClient.GetSeasonEntries(Season), null)
        );

        var sorted = (response?.Value ?? [])
            .OrderBy(e => e.Name)
            .ToList();

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            MediaEntries = new ObservableCollection<MediaEntry>(sorted);
        });
    }
    public async Task ItemTapped(MediaEntry mediaEntry)
    {
        await Shell.Current.GoToAsync(nameof(EditMediaEntryPage), false, new Dictionary<string, object>
        {
            { nameof(EditMediaEntryViewModel.MediaEntryId), mediaEntry.Id },
            { nameof(EditMediaEntryViewModel.MediaEntry), mediaEntry }
        });
    }

    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public override async Task OnAppearingAsync()
    {
        MediaEntries.Clear();
        Task.Run(async () =>
        {
            await Refresh();
        }).FireAndForget(ex => logger.LogError($"Error: {ex}"));
    }
}
