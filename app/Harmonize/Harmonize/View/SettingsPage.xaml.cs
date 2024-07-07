using Harmonize.ViewModel;
using AlohaKit.Animations;
using Microsoft.Maui.Controls;

namespace Harmonize.View;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();

		BindingContext = new SettingsViewModel();
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