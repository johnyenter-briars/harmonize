using Harmonize.Model;
using Harmonize.Page.View;
using Harmonize.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class LibraryViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public ICommand MoreInfoCommand => new Command<LocalMediaEntry>(entry =>
    {
    });

    private ObservableCollection<LocalMediaEntry> mediaEntries = [];
    public ObservableCollection<LocalMediaEntry> MediaEntries
    {
        get { return mediaEntries; }
        set { SetProperty(ref mediaEntries, value); }
    }

    async Task Refresh()
    {
        var media = await FetchData(async () =>
        {
            return await mediaManager.GetMediaEntries(Refresh);
        });

        MediaEntries.Clear();
        foreach (var m in media)
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
