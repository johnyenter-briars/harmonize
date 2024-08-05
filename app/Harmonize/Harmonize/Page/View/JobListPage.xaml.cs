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
        JobListViewModel  viewModel,
        MediaManager mediaManager
        ) : base(viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.mediaManager = mediaManager;
    }

    protected override async void OnAppearing()
    {
        await viewModel.PopulateJobs();
    }
    private void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is MediaEntry mediaEntry)
        {
            DisplayAlert("Media Tapped", $"You tapped on {mediaEntry.Name}", "OK");
        }
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is MediaEntry mediaEntry)
        {
            DisplayAlert("Play Media", $"Playing {mediaEntry.Name}", "OK");
        }
    }
}