using Harmonize.Model;
using Harmonize.Service;
using Harmonize.ViewModel;
using System.Collections.ObjectModel;

namespace Harmonize.Page.View;

public partial class MediaListPage : BasePage<MediaListViewModel>
{
    private readonly MediaListViewModel viewModel;
    private readonly MediaManager mediaManager;

    public MediaListPage(
        MediaListViewModel viewModel,
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
        if (e.Item is LocalMediaEntry mediaEntry)
        {
            await viewModel.ItemTapped(mediaEntry);
        }
    }
}