using Harmonize.Client;
using Harmonize.Client.Model.Season;
using Harmonize.Extensions;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class SeasonLibraryViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    ILogger<SeasonLibraryViewModel> logger,
    HarmonizeClient harmonizeCilent,
    AlertService alertService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    private Season selectedSeason;
    public Season SelectedSeason
    {
        get { return selectedSeason; }
        set { SetProperty(ref selectedSeason, value); }
    }
    public ICommand OpenBottomSheetCommand => new Command<Season>(entry =>
    {
        SelectedSeason = entry;
    });
    private List<string> options = ["foo", "bar"];
    public List<string> Options
    {
        get { return options; }
        set { SetProperty(ref options, value); }
    }
    private ObservableCollection<Season> seasons = [];
    public ObservableCollection<Season> Seasons
    {
        get { return seasons; }
        set { SetProperty(ref seasons, value); }
    }

    async Task Refresh()
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(harmonizeCilent.GetSeasons, null);
        });

        Seasons.Clear();
        foreach (var m in response?.Value ?? [])
        {
            Seasons.Add(m);
        }
    }
    public async Task ItemTapped(Season season)
    {
        //await Shell.Current.GoToAsync(nameof(EditMediaEntryPage), new Dictionary<string, object>
        //{
        //    { nameof(EditMediaEntryViewModel.MediaEntryId), season.Id },
        //    { nameof(EditMediaEntryViewModel.MediaEntry), season }
        //});
    }

    public override async Task OnAppearingAsync()
    {
        Task.Run(() =>
        {
            FetchingData = true;
        }).FireAndForget(ex => logger.LogError($"Error: {ex}"));

        await Task.CompletedTask;
    }
}
