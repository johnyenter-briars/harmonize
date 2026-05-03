using Harmonize.Client.Model.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Harmonize.Model;

public class SelectableMediaEntry(MediaEntry entry) : INotifyPropertyChanged
{
    private bool isSelected;

    public MediaEntry Entry { get; } = entry;
    public Guid Id => Entry.Id;
    public string Name => Entry.Name;
    public bool Transferred => Entry.Transferred;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected == value)
            {
                return;
            }

            isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
