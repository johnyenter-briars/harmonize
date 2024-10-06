using Harmonize.Client;
using Harmonize.Client.Model.QBT;
using Harmonize.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class ManageQbtViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService,
    AlertService alertService
) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    #region Bindings
    private bool orderByName;
    public bool OrderByName
    {
        get => orderByName;
        set
        {
            if (orderByName != value)
            {
                orderByName = value;
                OnPropertyChanged();

                if (orderByName)
                {
                    OrderByPercentage = false;
                }
            }
        }
    }
    private bool orderByPercentage;
    public bool OrderByPercentage
    {
        get => orderByPercentage;
        set
        {
            if (orderByPercentage != value)
            {
                orderByPercentage = value;
                OnPropertyChanged();

                if (orderByPercentage)
                {
                    OrderByName = false;
                }
            }
        }
    }
    private ObservableCollection<QbtDownloadData> activeDownloads = [];
    public ObservableCollection<QbtDownloadData> ActiveDownloads
    {
        get => activeDownloads;
        set => SetProperty(ref activeDownloads, value);
    }
    #endregion

    public ICommand RetrieveQbtDownloads => new Command(async () =>
    {
        await RefreshQbtDownloads();
    });
    public ICommand DeleteDownload => new Command<QbtDownloadData>(async (downloadData) =>
    {
        var (result, success) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.DeleteQbtDownloads(new DeleteDownloadsRequest
            {
                Hashes = [downloadData.Hash]
            });
        }, null);

        if (success)
        {
            await RefreshQbtDownloads();
        }

    });
    public ICommand PlayPauseDownload => new Command<QbtDownloadData>(async (downloadData) =>
    {
        if (downloadData.Active)
        {
            var (result, success) = await failsafeService.Fallback(async () =>
            {
                return await harmonizeClient.PauseQbtDownloads(new PauseDownloadsRequest
                {
                    Hashes = [downloadData.Hash]
                });
            }, null);

            if (success)
            {
                await RefreshQbtDownloads();
            }
        }
        else if (downloadData.InActive)
        {
            var (result, success) = await failsafeService.Fallback(async () =>
            {
                return await harmonizeClient.ResumeQbtDownloads(new ResumeDownloadsRequest
                {
                    Hashes = [downloadData.Hash]
                });
            }, null);
            
            if (success)
            {
                await RefreshQbtDownloads();
            }
        }
    });
    async Task RefreshQbtDownloads()
    {
        var (results, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () =>
            {
                return await harmonizeClient.GetQbtDownloads();
            }, null);
        });

        if (success)
        {
            ActiveDownloads.Clear();
            foreach (var download in results?.Value ?? [])
            {
                ActiveDownloads.Add(download);
            }
        }
    }

    public override async Task OnAppearingAsync()
    {
        await RefreshQbtDownloads();
    }
}

