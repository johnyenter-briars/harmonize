﻿using AlohaKit.Animations;
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
        Padding = 12;

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
            view.Animate(new StoryBoard(new List<AnimationBase>
              {
                 new ScaleToAnimation { Scale = 1.1, Duration = "150" },
                 new ScaleToAnimation { Scale = currentScale, Duration = "100" }
              }));
        }
    }
    void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
       
        //var selectedItem = e.SelectedItem as MagnetLinkSearchResult;

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
}
