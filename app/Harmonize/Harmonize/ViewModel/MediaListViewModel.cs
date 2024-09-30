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
        PreferenceManager preferenceManager,
        FailsafeService failsafeService
        ) : base(mediaManager, preferenceManager, failsafeService)
    {
        MediaEntries = [];

        Task.Run(PopulateEntries);
    }
    async Task PopulateEntries()
    {
        var (media, success) = await failsafeService.Fallback(mediaManager.GetMediaEntries, []);

        foreach (var m in media)
        {
            MediaEntries.Add(m);
        }
    }

    public override Task OnAppearingAsync()
    {
        throw new NotImplementedException();
    }
}
