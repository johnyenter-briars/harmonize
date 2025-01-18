using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Model;
using Harmonize.Page.View;
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

public class VideoLibraryViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    ILogger<VideoLibraryPage> logger,
    HarmonizeClient harmonizeCilent
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public ICommand MoreInfoCommand => new Command<MediaEntry>(entry =>
    {
    });
    public ICommand SendToMediaSystemCommand => new Command(() =>
    {
        var bing = SelectedMediaEntry;
        logger.LogInformation("foo");
    });
    private MediaEntry selectedMediaEntry;
    public MediaEntry SelectedMediaEntry
    {
        get { return selectedMediaEntry; }
        set { SetProperty(ref selectedMediaEntry, value); }
    }
    public ICommand OpenBottomSheetCommand => new Command<MediaEntry>(entry =>
    {
        SelectedMediaEntry = entry;
    });
    private List<string> options = ["foo", "bar"];
    public List<string> Options
    {
        get { return options; }
        set { SetProperty(ref options, value); }
    }
    private ObservableCollection<MediaEntry> mediaEntries = [];
    public ObservableCollection<MediaEntry> MediaEntries
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }

    async Task Refresh()
    {
        var (response, success) = await FetchData(async () =>
        {
            return await failsafeService.Fallback(harmonizeCilent.GetVideo, null);
        });

        MediaEntries.Clear();
        foreach (var m in response?.Value ?? [])
        {
            MediaEntries.Add(m);
        }
    }
    public async Task ItemTapped(LocalMediaEntry localMediaEntry)
    {
        await Shell.Current.GoToAsync(nameof(MediaElementPage), new Dictionary<string, object>
        {
            { nameof(MediaElementViewModel.MediaEntryId), localMediaEntry.Id }
        });
    }

    public override async Task OnAppearingAsync()
    {
        await Refresh();
    }
}
