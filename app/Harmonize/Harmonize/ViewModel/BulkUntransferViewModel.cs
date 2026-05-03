using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Transfer;
using Harmonize.Model;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class BulkUntransferViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    HarmonizeClient harmonizeClient,
    AlertService alertService,
    ILogger<BulkUntransferViewModel> logger
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private const int PageSize = 100;
    private readonly HarmonizeClient harmonizeClient = harmonizeClient;
    private readonly AlertService alertService = alertService;
    private readonly ILogger<BulkUntransferViewModel> logger = logger;

    private ObservableCollection<SelectableMediaEntry> entries = [];
    public ObservableCollection<SelectableMediaEntry> Entries
    {
        get => entries;
        set => SetProperty(ref entries, value);
    }

    private bool isSubmitting;
    public bool IsSubmitting
    {
        get => isSubmitting;
        set
        {
            if (SetProperty(ref isSubmitting, value))
            {
                OnPropertyChanged(nameof(HasEntriesAndNotSubmitting));
                OnPropertyChanged(nameof(CanSubmit));
            }
        }
    }

    private int selectedCount;
    public int SelectedCount
    {
        get => selectedCount;
        set
        {
            if (SetProperty(ref selectedCount, value))
            {
                OnPropertyChanged(nameof(SelectedCountText));
                OnPropertyChanged(nameof(SubmitButtonText));
                OnPropertyChanged(nameof(CanSubmit));
            }
        }
    }

    public bool HasEntries => Entries.Count > 0;
    public bool HasEntriesAndNotSubmitting => HasEntries && !IsSubmitting;
    public bool CanSubmit => SelectedCount > 0 && !IsSubmitting;
    public string SelectedCountText => SelectedCount == 0
        ? "No files selected"
        : $"{SelectedCount} file{(SelectedCount == 1 ? "" : "s")} selected";
    public string SubmitButtonText => SelectedCount == 0
        ? "Submit"
        : $"Submit ({SelectedCount})";

    public ICommand RefreshCommand => new Command(async () => await LoadTransferredEntries());
    public ICommand ToggleSelectionCommand => new Command<SelectableMediaEntry>(entry =>
    {
        if (entry is null || IsSubmitting)
        {
            return;
        }

        entry.IsSelected = !entry.IsSelected;
    });
    public ICommand SelectAllCommand => new Command(() =>
    {
        if (IsSubmitting)
        {
            return;
        }

        foreach (var entry in Entries)
        {
            entry.IsSelected = true;
        }
    });
    public ICommand ClearSelectionCommand => new Command(() =>
    {
        if (IsSubmitting)
        {
            return;
        }

        foreach (var entry in Entries)
        {
            entry.IsSelected = false;
        }
    });
    public ICommand SubmitCommand => new Command(async () => await Submit());

    public override async Task OnAppearingAsync()
    {
        await LoadTransferredEntries();
    }

    private async Task LoadTransferredEntries()
    {
        if (IsSubmitting)
        {
            return;
        }

        var loadedEntries = new List<MediaEntry>();
        var skip = 0;
        var hasMore = true;

        await FetchData(async () =>
        {
            while (hasMore)
            {
                var (response, success) = await failsafeService.Fallback(async () =>
                    await harmonizeClient.GetVideosPaging(
                        PageSize,
                        skip,
                        null,
                        [VideoType.Movie, VideoType.Episode],
                        true), null);

                var page = response?.Value?.ToList() ?? [];

                if (!success || page.Count == 0)
                {
                    hasMore = false;
                    break;
                }

                loadedEntries.AddRange(page);
                skip += PageSize;

                if (page.Count < PageSize)
                {
                    hasMore = false;
                }
            }

            return true;
        });

        foreach (var entry in Entries)
        {
            entry.PropertyChanged -= OnEntryPropertyChanged;
        }

        Entries.Clear();
        foreach (var entry in loadedEntries)
        {
            var selectableEntry = new SelectableMediaEntry(entry);
            selectableEntry.PropertyChanged += OnEntryPropertyChanged;
            Entries.Add(selectableEntry);
        }

        SelectedCount = 0;
        OnPropertyChanged(nameof(HasEntries));
        OnPropertyChanged(nameof(HasEntriesAndNotSubmitting));
    }

    private async Task Submit()
    {
        if (!CanSubmit)
        {
            return;
        }

        var selectedEntries = Entries.Where(entry => entry.IsSelected).ToList();

        if (selectedEntries.Count == 0)
        {
            return;
        }

        var confirmed = await alertService.ShowConfirmationAsync(
            "Bulk Untransfer",
            $"Untransfer {selectedEntries.Count} selected file{(selectedEntries.Count == 1 ? "" : "s")}?"); 

        if (!confirmed)
        {
            return;
        }

        IsSubmitting = true;
        var successCount = 0;

        try
        {
            foreach (var entry in selectedEntries)
            {
                var (response, success) = await failsafeService.Fallback(
                    async () => await harmonizeClient.Untransfer(TransferDestination.MediaSystem, entry.Entry), null);

                if (success && response?.Success == true)
                {
                    successCount++;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error bulk untransferring files");
        }
        finally
        {
            IsSubmitting = false;
        }

        if (successCount > 0)
        {
            await alertService.ShowAlertSnackbarAsync(
                successCount == selectedEntries.Count
                    ? $"Untransferred {successCount} file{(successCount == 1 ? "" : "s")}."
                    : $"Untransferred {successCount} of {selectedEntries.Count} files.");
        }
        else
        {
            await alertService.ShowAlertSnackbarAsync("No files were untransferred.");
        }

        await LoadTransferredEntries();
    }

    private void OnEntryPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectableMediaEntry.IsSelected))
        {
            SelectedCount = Entries.Count(entry => entry.IsSelected);
        }
    }
}
