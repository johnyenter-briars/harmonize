using Harmonize.Client.Model.Job;
using Harmonize.Model;
using Harmonize.Service;
using Harmonize.ViewModel;
using System.Collections.ObjectModel;

namespace Harmonize.Page.View;

public partial class JobListPage : BasePage<JobListViewModel>
{
    private readonly JobListViewModel viewModel;
    private readonly MediaManager mediaManager;

    public JobListPage(
        JobListViewModel viewModel,
        MediaManager mediaManager
        ) : base(viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.mediaManager = mediaManager;
    }

    protected override async void OnAppearing()
    {
        await viewModel.OnAppearingAsync();
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Job job)
        {
            await viewModel.ItemTapped(job);
        }
    }
}