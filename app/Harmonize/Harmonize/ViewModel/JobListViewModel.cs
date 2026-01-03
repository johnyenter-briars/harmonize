using Harmonize.Client;
using Harmonize.Client.Model.Job;
using Harmonize.Extensions;
using Harmonize.Page.View;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class JobListViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService,
    ILogger<JobListViewModel> logger
        ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private readonly HarmonizeClient harmonizeClient = harmonizeClient;
    #region Bindings
    private ObservableCollection<Job> jobs = [];
    public ObservableCollection<Job> Jobs
    {
        get { return jobs; }
        set { SetProperty(ref jobs, value); }
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
            return await failsafeService.Fallback(harmonizeClient.GetJobs, null);
        });

        Jobs.Clear();

        if (success)
        {
            var jobs = (response?.Value ?? []).OrderByDescending(j => j.StartedOn);
            foreach (var j in jobs)
            {
                Jobs.Add(j);
            }
        }
    }
    public async Task ItemTapped(Job job)
    {
        if (job != null)
        {
            await Shell.Current.GoToAsync(nameof(EditJobPage), false, new Dictionary<string, object>
            {
                { nameof(EditJobViewModel.JobId), job.Id }
            });
        }
    }
    public override async Task OnAppearingAsync()
    {
        Task.Run(() =>
        {
            FetchingData = true;
        }).FireAndForget(ex => logger.LogError($"Error: {ex}"));

        await Task.CompletedTask;
    }
}
