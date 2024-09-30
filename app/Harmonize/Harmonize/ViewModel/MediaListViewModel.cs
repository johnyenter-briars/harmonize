using Harmonize.Model;
using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.ViewModel;

public class MediaListViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private ObservableCollection<MediaEntry> mediaEntries = [];
    public ObservableCollection<MediaEntry> MediaEntries
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }

    async Task PopulateEntries()
    {
        var media = await mediaManager.GetMediaEntries();

        foreach (var m in media)
        {
            MediaEntries.Add(m);
        }
    }

    public override async Task OnAppearingAsync()
    {
        await PopulateEntries();
    }
}
