﻿using CommunityToolkit.Maui.Core;
using Harmonize.Client;
using Harmonize.Client.Model.Job;
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
    #region Bindings
    private ObservableCollection<Job> jobs = [];
    private readonly HarmonizeClient harmonizeClient = harmonizeClient;

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

    public async Task PopulateJobs()
    {
        Jobs.Clear();

        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(harmonizeClient.GetJobs, null);
        });
            
        if (success)
        {
            foreach (var m in response?.Value ?? [])
            {
                Jobs.Add(m);
            }
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

    public ICommand Refresh => new Command<ImageButton>(async (imageButton) =>
    {
        await imageButton.RotateTo(100, 300, Easing.CubicInOut);
        await imageButton.RotateTo(0, 300, Easing.CubicInOut);

        await PopulateJobs();
    });
}
