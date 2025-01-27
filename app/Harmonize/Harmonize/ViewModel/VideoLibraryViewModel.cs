﻿using Harmonize.Client;
using Harmonize.Client.Model.Media;
using Harmonize.Client.Model.Transfer;
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
    HarmonizeClient harmonizeCilent,
    AlertService alertService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    public ICommand RefreshCommand => new Command(async () => await Refresh());
    public ICommand MoreInfoCommand => new Command<MediaEntry>(entry =>
    {
    });
    public ICommand SendToMediaSystemCommand => new Command(async () =>
    {
        var (jobResponse, success) = await failsafeService.Fallback(
            async () => await harmonizeCilent.StartTransfer(TransferDestination.MediaSystem, SelectedMediaEntry), null);

        if (success)
        {
            await alertService.ShowConfirmationAsync("Success", "Job created successfully.", "Ok");
        }
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
    public async Task ItemTapped(MediaEntry mediaEntry)
    {
        await Shell.Current.GoToAsync(nameof(EditMediaEntryPage), new Dictionary<string, object>
        {
            { nameof(EditMediaEntryViewModel.MediaEntryId), mediaEntry.Id },
            { nameof(EditMediaEntryViewModel.MediaEntry), mediaEntry }
        });
    }

    public override async Task OnAppearingAsync()
    {
        FetchingData = true;
    }
}
