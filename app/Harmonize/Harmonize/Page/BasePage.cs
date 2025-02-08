using AlohaKit.Animations;
using Harmonize.ViewModel;
using System.Diagnostics;

namespace Harmonize.Page;

public abstract class BasePage<TViewModel> : BasePage where TViewModel : BaseViewModel
{
    protected BasePage(TViewModel viewModel) : base(viewModel)
    {
    }

    public new TViewModel BindingContext => (TViewModel)base.BindingContext;
}

public abstract class BasePage : ContentPage
{
    protected BasePage(object? viewModel = null)
    {
        BindingContext = viewModel;
        Padding = 5;

        if (string.IsNullOrWhiteSpace(Title))
        {
            Title = GetType().Name;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Debug.WriteLine($"OnAppearing: {Title}");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Debug.WriteLine($"OnDisappearing: {Title}");
    }
    protected void ScaleButton(object sender, EventArgs e)
    {
        if (sender is Microsoft.Maui.Controls.View view)
        {
            var currentScale = view.Scale;
            var targetScale = currentScale * 1.1;
            view.Animate(new StoryBoard(new List<AnimationBase>
              {
                 new ScaleToAnimation { Scale = targetScale, Duration = "150" },
                 new ScaleToAnimation { Scale = currentScale, Duration = "100" }
              }));
        }
    }
    public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
       
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
            selectedViewCell.View.BackgroundColor = null;
        }

        listView.SelectedItem = null;
    }
    public void OnItemSelected(object sender, TappedEventArgs e)
    {
        if (e.Parameter == null)
            return;

        var bar = (Grid)sender;
        var collectionView = (CollectionView)bar.Parent.Parent;

        foreach (ViewCell viewCell in collectionView.ItemsSource)
        {
            if (viewCell != null)
            {
                viewCell.View.BackgroundColor = null;
            }
        }
        //var selectedViewCell = listView.TemplatedItems[e.SelectedItemIndex] as ViewCell;

        //if (selectedViewCell != null)
        //{
        //    selectedViewCell.View.BackgroundColor = null;
        //}

        //listView.SelectedItem = null;
    }
}
