using Harmonize.ViewModel;
using AlohaKit.Animations;
using Microsoft.Maui.Controls;
using Harmonize.Service;

namespace Harmonize.Page.View;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager
        )
	{
		InitializeComponent();

		BindingContext = new SettingsViewModel(mediaManager, preferenceManager);
	}
	private void ScaleButton(object sender, EventArgs e)
    {
        if (sender is Button view)
        {
            view.Animate(new StoryBoard(new List<AnimationBase>
              {
                 new ScaleToAnimation { Scale = 1.1, Duration = "150" },
                 new ScaleToAnimation { Scale = 1, Duration = "100" }
              }));
        }
    }
}