using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Transfer;
using Harmonize.Extensions;
using Harmonize.Page.View;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(MediaEntryId), nameof(MediaEntryId))]
[QueryProperty(nameof(MediaEntry), nameof(MediaEntry))]
public class EditMediaEntryViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    HarmonizeClient harmonizeClient,
    AlertService alertService,
    ILogger<EditMediaEntryViewModel> logger
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand DeleteEntry => new Command(async () =>
    {
        if (await alertService.ShowConfirmationAsync("Delete Entry", $"Are you sure you want to delete entry: {MediaEntry.Name}?") == true)
        {
            var (response, success) = await failsafeService.Fallback(async () => await harmonizeClient.DeleteEntry(MediaEntry), null);

            if (response?.Success == true)
            {
                await alertService.ShowAlertSnackbarAsync("Entry deleted.");
                await Shell.Current.GoToAsync("..", false);
            }
        }
    });
    public ICommand UntransferEntry => new Command(async () =>
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.Untransfer(TransferDestination.MediaSystem, MediaEntry), null);
        });

        if (response?.Success == true)
        {
            await alertService.ShowAlertSnackbarAsync("Removed file successfully.");
        }
    });
    public ICommand SaveEntry => new Command(async () =>
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.UpdateEntry(MediaEntry, new UpsertMediaEntryRequest { Name = MediaEntry.Name }), null);
        });
    });
    public ICommand SendEntry => new Command(async () =>
    {
        var (jobResponse, success) = await failsafeService.Fallback(
            async () => await harmonizeClient.StartTransfer(TransferDestination.MediaSystem, MediaEntry), null);

        if (success)
        {
            await alertService.ShowAlertSnackbarAsync("Job created successfully.");
        }
    });
    public ICommand SendSubtitleEntry => new Command<MediaEntry>(async (subtitleEntry) =>
    {
        var (jobResponse, success) = await failsafeService.Fallback(
            async () => await harmonizeClient.StartTransfer(TransferDestination.MediaSystem, subtitleEntry), null);

        if (success)
        {
            await alertService.ShowAlertSnackbarAsync("Job created successfully.");
        }
    });
    public ICommand NavigateToSeason => new Command(async () =>
    {
        if (MediaEntry.SeasonId is not null)
        {
            var (seasonResponse, success) = await failsafeService.Fallback(
                async () => await harmonizeClient.GetSeason((Guid)MediaEntry.SeasonId), null);

            if (success)
            {
                await Shell.Current.GoToAsync(nameof(EditSeasonPage), false, new Dictionary<string, object>
                {
                    { nameof(EditSeasonViewModel.Season), seasonResponse!.Value! }
                });
            }
        }
    });
    private ObservableCollection<MediaElementSource> sourceOptions =
    [
        MediaElementSource.Youtube,
        MediaElementSource.MagnetLink,
    ];
    public ObservableCollection<MediaElementSource> SourceOptions
    {
        get => sourceOptions;
        set
        {
            if (sourceOptions != value)
            {
                sourceOptions = value;
                OnPropertyChanged(nameof(SourceOptions));
            }
        }
    }
    private bool listViewVisible = true;
    public bool ListViewVisible
    {
        get => listViewVisible;
        set => SetProperty(ref listViewVisible, value);
    }
    private Guid mediaEntryId;
    public Guid MediaEntryId
    {
        get => mediaEntryId;
        set => SetProperty(ref mediaEntryId, value);
    }
    private MediaEntry mediaEntry;
    public MediaEntry MediaEntry
    {
        get => mediaEntry;
        set => SetProperty(ref mediaEntry, value);
    }
    private ObservableCollection<MediaEntry> subtitles = [];
    public ObservableCollection<MediaEntry> Subtitles
    {
        get { return subtitles; }
        set { SetProperty(ref subtitles, value); }
    }
    async Task Refresh()
    {
        if (MediaEntry.Type == MediaEntryType.Subtitle)
        {
            ListViewVisible = false;
            return;
        }

        ListViewVisible = true;

        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetSubsForVideo(MediaEntry), null);
        });

        Subtitles.Clear();
        foreach (var m in response?.Value ?? [])
        {
            Subtitles.Add(m);
        }
    }
    public override async Task OnAppearingAsync()
    {
        Task.Run(async () =>
        {
            if (MediaEntry == null)
            {
                var (response, success) = await failsafeService.Fallback(async () =>
                    await harmonizeClient.GetMediaEntry(MediaEntryId), null);

                if (success && response?.Value is not null)
                {
                    MediaEntry = response.Value;
                    await Refresh();
                }
                else
                {
                    await alertService.ShowAlertAsync("Error in getting entry", response?.Message ?? "Failure to perform action");
                }
            }
            else
            {
                await Refresh();
            }

        }).FireAndForget(ex => logger.LogError($"Error: {ex}"));

        await Task.CompletedTask;
    }

    internal async Task ItemTapped(MediaEntry mediaEntry)
    {
        //TODO: bug in navigating to the same type of page from the same type of page. Keeps the old binding. Idc rn
        await Shell.Current.GoToAsync(nameof(EditMediaEntryPage), false, new Dictionary<string, object>
        {
            { nameof(EditMediaEntryViewModel.MediaEntryId), mediaEntry.Id },
            { nameof(EditMediaEntryViewModel.MediaEntry), mediaEntry }
        });
    }
}
