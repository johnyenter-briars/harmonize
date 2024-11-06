using AlohaKit.Animations;
using Harmonize.Service;
using Harmonize.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace Harmonize.Page.View;
public partial class MediaControlPage : BasePage<MediaControlViewModel>
{
    private readonly MediaControlViewModel viewModel;
    readonly ILogger logger;
    private readonly MediaManager mediaManager;

    public MediaControlPage(
        MediaControlViewModel viewModel,
        ILogger<HomePage> logger,
        MediaManager mediaManager
        ) : base(viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.logger = logger;
        this.mediaManager = mediaManager;
    }
    private void VolumeSlider_Changed(object sender, ValueChangedEventArgs e)
    {
        var slider = (Slider)sender;
        double distanceFromMin = (slider.Value - slider.Minimum);
        double sliderRange = (slider.Maximum - slider.Minimum);
        double sliderPercent = 100 * (distanceFromMin / sliderRange);

        int sliderPercentAsint = (int)sliderPercent;

        //Task.Run(async () => await kodiClient.SetVolumeAsync(sliderPercentAsint));

    }
    private void SeekSlider_Changed(object sender, ValueChangedEventArgs e)
    {
        var slider = (Slider)sender;
        double distanceFromMin = (slider.Value - slider.Minimum);
        double sliderRange = (slider.Maximum - slider.Minimum);
        double sliderPercent = 100 * (distanceFromMin / sliderRange);

        int sliderPercentAsint = (int)sliderPercent;

        //Task.Run(async () => await kodiClient.SeekPlayerAsync(sliderPercentAsint));
    }
    private void ScaleButton(object sender, EventArgs e)
    {
        if (sender is Microsoft.Maui.Controls.View view)
        {
            view.Animate(new StoryBoard(new List<AnimationBase>
              {
                 new ScaleToAnimation { Scale = 1.1, Duration = "150" },
                 new ScaleToAnimation { Scale = 1, Duration = "100" }
              }));
        }
    }

}
