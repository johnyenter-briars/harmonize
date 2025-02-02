using Harmonize.Client;
using Harmonize.Client.Model.Job;
using Harmonize.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(JobId), nameof(JobId))]
public class EditJobViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService,
    AlertService alertService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private ObservableCollection<JobStatus> statusOptions =
    [
        JobStatus.Succeeded,
        JobStatus.Running,
        JobStatus.Failed,
        JobStatus.Canceled
    ];
    public ObservableCollection<JobStatus> StatusOptions
    {
        get => statusOptions;
        set
        {
            if (statusOptions != value)
            {
                statusOptions = value;
                OnPropertyChanged(nameof(StatusOptions));
            }
        }
    }
    private Guid jobId;
    public Guid JobId
    {
        get => jobId;
        set
        {
            if (jobId != value)
            {
                jobId = value;
                OnPropertyChanged(nameof(JobId));
            }
        }
    }
    private Job? job;
    public Job? Job
    {
        get => job;
        set
        {
            if (job != value)
            {
                job = value;
                OnPropertyChanged(nameof(Job));
            }
        }
    }

    public async Task RetrieveJob()
    {
        var (response, success) = await failsafeService.Fallback(async () =>
        {
            return await harmonizeClient.GetJob(JobId);
        }, null);

        if (success)
        {
            Job = response?.Value;
        }
    }

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }

    public ICommand CancelJob => new Command<Job>(async (job) =>
    {
        if (job != null)
        {
            var (_, success) = await failsafeService.Fallback(async () =>
            {
                return await harmonizeClient.CancelJob(job.Id);
            }, null);

            if (success)
            {
                await alertService.ShowAlertSnackbarAsync("Job canceled successfully.");
            }
        }
    });
}