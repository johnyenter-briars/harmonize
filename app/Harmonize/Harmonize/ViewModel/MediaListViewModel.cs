using Harmonize.Model;
using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.ViewModel;

public class MediaListViewModel : BaseViewModel
{
    private ObservableCollection<MediaEntry> mediaEntries  = [];
    public ObservableCollection<MediaEntry> MediaEntries 
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }

    public MediaListViewModel(
        MediaManager mediaManager,
        PreferenceManager preferenceManager
        ) : base(mediaManager, preferenceManager)
    {
        MediaEntries = [];

        Task.Run(PopulateEntries);
    }
    async Task PopulateEntries()
    {
        var media = await mediaManager.GetMediaEntries();

        foreach (var m in media)
        {
            MediaEntries.Add(m);
        }
    }
}
