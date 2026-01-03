using Harmonize.Model;
using Harmonize.Page.View;
using Harmonize.Service;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class AudioLibraryViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    ILogger<VideoLibraryPage> logger
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public ICommand MoreInfoCommand => new Command<LocalMediaEntry>(entry =>
    {
    });
    private List<string> options = ["foo", "bar"];
    public List<string> Options 
    {
        get { return options; }
        set { SetProperty(ref options, value); }
    }
    private ObservableCollection<LocalMediaEntry> mediaEntries = [];
    public ObservableCollection<LocalMediaEntry> MediaEntries
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }

    async Task Refresh()
    {
        if (FetchingData)
        {
            logger.LogInformation($"{nameof(FetchingData)} is true, not fetching data again");
            return;
        }

        var media = await FetchData(mediaManager.GetAudioMediaEntries);

        MediaEntries.Clear();
        foreach (var m in media)
        {
            MediaEntries.Add(m);
        }
    }
    public async Task ItemTapped(LocalMediaEntry localMediaEntry)
    {
        await Shell.Current.GoToAsync(nameof(MediaElementPage), false, new Dictionary<string, object>
        {
            { nameof(MediaElementViewModel.MediaEntryId), localMediaEntry.Id }
        });
    }

    public override async Task OnAppearingAsync()
    {
        //TODO: switch this for fetchindata = true
        await Refresh();
    }
}
