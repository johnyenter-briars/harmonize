using Harmonize.Client.Model.QBT;
using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Page.View;

public partial class ManageQbtPage : BasePage<ManageQbtViewModel>
{
    private readonly ManageQbtViewModel viewModel;

    public ManageQbtPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        ManageQbtViewModel viewModel
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ManageQbtViewModel viewModel)
        {
            await viewModel.OnAppearingAsync();
        }
    }
    private async void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        //if (e.Item is MagnetLinkSearchResult magnetlinkSearchResult)
        //{
        //    await viewModel.ItemTapped(magnetlinkSearchResult);
        //}
    }
    void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var selectedItem = e.SelectedItem as QbtDownloadData;

        var listView = sender as ListView;

        foreach (ViewCell viewCell in listView.TemplatedItems)
        {
            if (viewCell != null)
            {
                viewCell.View.BackgroundColor = null;
            }
        }

        var selectedViewCell = listView.TemplatedItems[e.SelectedItemIndex] as ViewCell;

        if (selectedViewCell != null)
        {
            //selectedViewCell.View.BackgroundColor = (Color)Application.Current.Resources["Primary"]; ;
            selectedViewCell.View.BackgroundColor = null;
        }

        listView.SelectedItem = null;
    }
}