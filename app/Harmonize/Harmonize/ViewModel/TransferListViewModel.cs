using Harmonize.Client;
using Harmonize.Client.Model.Transfer;
using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class TransferListViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService
        ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private readonly HarmonizeClient harmonizeClient = harmonizeClient;
    #region Bindings
    private ObservableCollection<TransferProgress> transferProgress = [];
    public ObservableCollection<TransferProgress> TransferProgress
    {
        get { return transferProgress; }
        set { SetProperty(ref transferProgress, value); }
    }
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
                    OrderByStatus = false;
                }
            }
        }
    }
    private bool orderByStatus;
    public bool OrderByStatus
    {
        get => orderByStatus;
        set
        {
            if (orderByStatus != value)
            {
                orderByStatus = value;
                OnPropertyChanged();

                if (orderByStatus)
                {
                    OrderByName = false;
                }
            }
        }
    }
    #endregion

    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public async Task Refresh()
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetTransferProgress(TransferDestination.MediaSystem), null);
        });

        TransferProgress.Clear();

        if (success)
        {
            var jobs = (response?.Value ?? []).OrderByDescending(j => j.StartTime);
            foreach (var j in jobs)
            {
                TransferProgress.Add(j);
            }
        }
    }
    public async Task ItemTapped(TransferProgress transferProgress)
    {
        if (transferProgress != null)
        {
        }
    }
    public override async Task OnAppearingAsync()
    {
        FetchingData = true;
    }
}
