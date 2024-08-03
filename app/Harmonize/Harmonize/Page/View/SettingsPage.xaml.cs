using Harmonize.ViewModel;
using AlohaKit.Animations;
using Microsoft.Maui.Controls;
using Harmonize.Service;

namespace Harmonize.Page.View;

public partial class SettingsPage : BasePage<SettingsViewModel>
{
	public SettingsPage(
        MediaManager mediaManager,
        PreferenceManager preferenceManager,
        SettingsViewModel viewModel
        ) : base(viewModel)
	{
		InitializeComponent();
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