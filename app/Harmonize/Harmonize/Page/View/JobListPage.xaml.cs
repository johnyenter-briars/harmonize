using Harmonize.Client.Model.Job;
using Harmonize.Service;
using Harmonize.ViewModel;

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
    private async void OnFilterClicked(object sender, EventArgs e)
    {
        base.ScaleButton(sender, e);

        if(!filterMenu.IsVisible)
        {
            await filterMenu.ShowAsync();
        }
        else
        {
            await filterMenu.HideAsync();
        }
    }
}