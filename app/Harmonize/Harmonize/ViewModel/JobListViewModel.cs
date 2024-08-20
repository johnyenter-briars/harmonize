using Harmonize.Client;
using Harmonize.Client.Model.System;
using Harmonize.Model;
using Harmonize.Page.View;
using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class JobListViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient
        ) : BaseViewModel(mediaManager, preferenceManager)
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

        var jobs = await harmonizeClient.GetJobs();

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

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }

    public ICommand Refresh => new Command<Job>(async (job) =>
    {
        await PopulateJobs();
    });
}
