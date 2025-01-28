using Harmonize.Client;
using Harmonize.Client.Model.Health;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class HealthViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    HarmonizeClient harmonizeClient,
    AlertService alertService,
    ILogger<HealthViewModel> logger
) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    private HealthStatus healthStatus;
    public HealthStatus HealthStatus
    {
        get => healthStatus;
        set => SetProperty(ref healthStatus, value);
    }
    async Task Refresh()
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(harmonizeClient.GetHealthStatus, null);
        });

        if (success && response.Value is not null)
        {
            HealthStatus = response.Value;
        }
        else
        {
            await alertService.ShowAlertAsync("Error in getting health check", response?.Message ?? "Failure to perform action");
        }
    }
    public override async Task OnAppearingAsync()
    {
        FetchingData = true;
    }
}
