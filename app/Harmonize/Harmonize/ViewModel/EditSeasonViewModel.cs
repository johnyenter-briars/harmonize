using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Season;
using Harmonize.Extensions;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmonize.ViewModel;

[QueryProperty(nameof(Seaon), nameof(Seaon))]
public class EditSeasonViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    HarmonizeClient harmonizeClient,
    ILogger<EditSeasonViewModel> logger
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private ObservableCollection<MediaEntry> mediaEntries = [];
    public ObservableCollection<MediaEntry> MediaEntries
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }
    private Season season;
    public Season Seaon
    {
        get => season;
        set => SetProperty(ref season, value);
    }
    public ICommand DeleteSeason => new Command<Season>(async (entry) =>
    {
    });
    public ICommand SaveSeason => new Command<Season>(async (entry) =>
    {
    });
    async Task Refresh()
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(async () => await harmonizeClient.GetSeasonEntries(Seaon), null);
        });

        MediaEntries.Clear();
        foreach (var m in response?.Value ?? [])
        {
            MediaEntries.Add(m);
        }
    }
    public async Task ItemTapped(MediaEntry mediaEntry)
    {
        var foo = 10;
    }

    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public override async Task OnAppearingAsync()
    {
        Task.Run(async () =>
        {
            await Refresh();
        }).FireAndForget(ex => logger.LogError($"Error: {ex}"));
    }
}
