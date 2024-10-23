using Harmonize.Model;
using Harmonize.Page.View;
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
    private ObservableCollection<LocalMediaEntry> mediaEntries = [];
    public ObservableCollection<LocalMediaEntry> MediaEntries
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }

    async Task PopulateEntries()
    {
        var media = await mediaManager.GetMediaEntries();

        MediaEntries.Clear();
        foreach (var m in media)
        {
            MediaEntries.Add(m);
        }
    }
    public async Task MediaEntryTapped(LocalMediaEntry localMediaEntry)
    {
        await Shell.Current.GoToAsync(nameof(MediaElementPage), new Dictionary<string, object>
        {
            { nameof(MediaElementViewModel.MediaEntryId), localMediaEntry.Id }
        });
    }

    public override async Task OnAppearingAsync()
    {
        await PopulateEntries();
    }
}
