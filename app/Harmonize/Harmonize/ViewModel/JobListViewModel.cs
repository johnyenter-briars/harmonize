using CommunityToolkit.Maui.Core;
using Harmonize.Client;
using Harmonize.Client.Model.System;
using Harmonize.Page.View;
using Harmonize.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class JobListViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService
        ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private ObservableCollection<Job> jobs = [];
    private readonly HarmonizeClient harmonizeClient = harmonizeClient;

    public ObservableCollection<Job> Jobs
    {
        get { return jobs; }
        set { SetProperty(ref jobs, value); }
    }

    public async Task PopulateJobs()
    {
        Jobs.Clear();

        var (jobs, success) = await failsafeService.Fallback(harmonizeClient.GetJobs, []);

        foreach (var m in jobs)
        {
            Jobs.Add(m);
        }
    }
    public async Task ItemTapped(Job job)
    {
        if (job != null)
        {
            await Shell.Current.GoToAsync(nameof(EditJobPage), new Dictionary<string, object>
            {
                { nameof(EditJobViewModel.JobId), job.Id }
            });
        }
    }

    public override async Task OnAppearingAsync()
    {
        await PopulateJobs();
    }

    public ICommand Refresh => new Command<Job>(async (job) =>
    {
        await PopulateJobs();
    });
}
