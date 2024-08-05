using Harmonize.Client;
using Harmonize.Client.Model.System;
using Harmonize.Model;
using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class JobListViewModel : BaseViewModel
{
    public JobListViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        HarmonizeClient harmonizeClient
        ) : base(mediaManager, preferenceManager)
    {
        this.harmonizeClient = harmonizeClient;
    }

    private ObservableCollection<Job> jobs = [];
    private readonly HarmonizeClient harmonizeClient;

    public ObservableCollection<Job> Jobs
    {
        get { return jobs; }
        set { SetProperty(ref jobs, value); }
    }

    public async Task PopulateJobs()
    {
        Jobs.Clear();

        var jobs = await harmonizeClient.GetJobs();

        foreach (var m in jobs)
        {
            Jobs.Add(m);
        }
    }
    public ICommand CancelJob => new Command<Job>(async (job) =>
    {
        var _ = await harmonizeClient.CancelJob(job.Id);
    });
    public ICommand Refresh => new Command<Job>(async (job) =>
    {
        await PopulateJobs();
    });
}
