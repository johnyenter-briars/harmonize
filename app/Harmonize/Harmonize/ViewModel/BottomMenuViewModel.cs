using Harmonize.Model;
using Harmonize.Client;
using Harmonize.Client.Model.Job;
using Harmonize.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Harmonize.ViewModel;

public class BottomMenuViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    HarmonizeClient harmonizeClient,
    FailsafeService failsafeService,
    AlertService alertService
    ) : BaseViewModel(mediaManager, preferenceManager, failsafeService)
{
    private LocalMediaEntry _selectedMediaEntry;

    public LocalMediaEntry SelectedMediaEntry
    {
        get => _selectedMediaEntry;
        set
        {
            _selectedMediaEntry = value;
            OnPropertyChanged(); // Notify the UI when the property changes
        }
    }

    public ICommand AddToPlaylistCommand { get; }
    public ICommand AddToQueueCommand { get; }
    public ICommand SyncCommand { get; }
    public ICommand CancelCommand { get; }

    private void OnAddToPlaylist()
    {
        // Handle Add to Playlist action
        // You can now access SelectedMediaEntry in this method
    }

    private void OnAddToQueue()
    {
        // Handle Add to Queue action
    }

    private void OnSync()
    {
        // Handle Sync action
    }

    private void OnCancel()
    {
        // Handle Cancel action
    }

    public override Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }
}
