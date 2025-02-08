using Harmonize.Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Harmonize.ViewModel;

public abstract partial class BaseViewModel(
    MediaManager mediaManager,
    PreferenceManager preferenceManager,
    FailsafeService failsafeService
        ) : INotifyPropertyChanged
{
    private bool fetchingData = false;
    public bool FetchingData
    {
        get { return fetchingData; }
        set
        {
            SetProperty(ref fetchingData, value);
        }
    }
    private bool notFetchingData = true;
    public bool NotFetchingData
    {
        get { return notFetchingData; }
        set { SetProperty(ref notFetchingData, value); }
    }

    string title = string.Empty;
    protected readonly MediaManager mediaManager = mediaManager;
    protected readonly PreferenceManager preferenceManager = preferenceManager;
    protected readonly FailsafeService failsafeService = failsafeService;

    public string Title
    {
        get { return title; }
        set { SetProperty(ref title, value); }
    }
    //TODO: This has weird behavior with refresh view for some reason... sometimes... idk
    protected async Task<T> FetchData<T>(Func<Task<T>> callback)
    {
        FetchingData = true;
        NotFetchingData = false;

        T response = await callback();

        FetchingData = false;
        NotFetchingData = true;

        return response;
    }

    protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action? onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    public abstract Task OnAppearingAsync();

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
