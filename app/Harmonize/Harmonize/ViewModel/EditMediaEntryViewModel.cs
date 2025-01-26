﻿using Harmonize.Client.Model.Media;
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

[QueryProperty(nameof(MediaEntryId), nameof(MediaEntryId))]
[QueryProperty(nameof(MediaEntry), nameof(MediaEntry))]
public class EditMediaEntryViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService,
    ILogger<EditMediaEntryViewModel> logger
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private ObservableCollection<MediaElementSource> sourceOptions =
    [
        MediaElementSource.Youtube,
        MediaElementSource.MagnetLink,
    ];
    public ObservableCollection<MediaElementSource> SourceOptions
    {
        get => sourceOptions;
        set
        {
            if (sourceOptions != value)
            {
                sourceOptions  = value;
                OnPropertyChanged(nameof(SourceOptions));
            }
        }
    }
    private Guid mediaEntryId;
    public Guid MediaEntryId
    {
        get => mediaEntryId;
        set => SetProperty(ref mediaEntryId, value);
    }
    private MediaEntry mediaEntry;
    public MediaEntry MediaEntry
    {
        get => mediaEntry;
        set => SetProperty(ref mediaEntry, value);
    }
    public ICommand DeleteEntry => new Command<MediaEntry>(async (entry) =>
    {
        //var (_, success) = await failsafeService.Fallback(async () =>
        //{
        //    return await harmonizeClient.CancelJob(entry.Id);
        //}, null);

        //if (success)
        //{
        //    await alertService.ShowConfirmationAsync("Success", "Job canceled successfully.", "Ok");
        //}
    });
    async Task Refresh()
    {
        //var (response, success) = await FetchData(async () =>
        //{
        //    return await failsafeService.Fallback(harmonizeCilent.GetVideo, null);
        //});

        //MediaEntries.Clear();
        //foreach (var m in response?.Value ?? [])
        //{
        //    MediaEntries.Add(m);
        //}
    }
    public override async Task OnAppearingAsync()
    {
        var foo = 10;
    }
}
