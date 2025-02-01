using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Transfer;
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
    public ICommand DeleteEntry => new Command<MediaEntry>(async (entry) =>
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.DeleteEntry(MediaEntry), null);
        });

        if (response.Success)
        {
            await Shell.Current.GoToAsync("..");
        }
    });
    public ICommand SaveEntry => new Command<MediaEntry>(async (entry) =>
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.UpdateEntry(MediaEntry, new UpsertMediaEntryRequest { Name = MediaEntry.Name }), null);
        });
    });
    public ICommand SendEntry => new Command<MediaEntry>(async (entry) =>
    {
        var (jobResponse, success) = await failsafeService.Fallback(
            async () => await harmonizeClient.StartTransfer(TransferDestination.MediaSystem, MediaEntry), null);

        if (success)
        {
            await alertService.ShowConfirmationAsync("Success", "Job created successfully.", "Ok");
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
    async Task Refresh()
    {
    }
    public override async Task OnAppearingAsync()
    {
    }
}
