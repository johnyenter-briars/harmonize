using Harmonize.Client;
using Harmonize.Client.Model.System;
using Harmonize.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(JobId), nameof(JobId))]
public class EditJobViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient
    ) : BaseViewModel(mediaManager, preferenceManager)
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
        Job = await harmonizeClient.GetJob(JobId);
    }

    public ICommand CancelJob => new Command<Job>(async (job) =>
    {
        if (job != null)
        {
            var mainPage = Application.Current?.MainPage;
            if (mainPage != null)
            {
                await harmonizeClient.CancelJob(job.Id);

                await mainPage.DisplayAlert("Success", "Job updated successfully.", "OK");
            }
        }
    });
}