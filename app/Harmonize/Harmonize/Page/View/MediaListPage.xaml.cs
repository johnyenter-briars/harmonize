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
    private void OnMediaItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is MediaEntry mediaEntry)
        {
            DisplayAlert("Media Tapped", $"You tapped on {mediaEntry.Name}", "OK");
        }
    }

    private void OnPlayButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is MediaEntry mediaEntry)
        {
            DisplayAlert("Play Media", $"Playing {mediaEntry.Name}", "OK");
        }
    }
}