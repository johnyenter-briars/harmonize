using CommunityToolkit.Maui.Views;
using Harmonize.Client;
using Harmonize.Client.Model.Season;
using Harmonize.Service;
using Harmonize.ViewModel;

namespace Harmonize.Components;

public partial class CreateSeasonPopup : Popup
{
    private readonly HarmonizeClient harmonizeClient;
    private readonly FailsafeService failsafeService;

    public CreateSeasonPopup(
        HarmonizeClient harmonizeClient,
        FailsafeService failsafeService
        )
    {
        InitializeComponent();
        this.harmonizeClient = harmonizeClient;
        this.failsafeService = failsafeService;
    }
    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        var name = seasonName.Text;
        var foo = await failsafeService.Fallback(async () => 
        await harmonizeClient.CreateSeason(new CreateSeasonRequest { 
            Name = name,
        }), null);

        await CloseAsync();
    }
}