namespace Harmonize.Components;

public partial class ListElement : ContentView
{
    public static readonly BindableProperty CustomPaddingProperty =
    BindableProperty.Create(
        nameof(CustomPadding),
        typeof(Thickness),
        typeof(ListElement),
        new Thickness(10), // Default padding
        propertyChanged: OnPaddingChanged);

    public Thickness CustomPadding
    {
        get => (Thickness)GetValue(CustomPaddingProperty);
        set => SetValue(CustomPaddingProperty, value);
    }

    private static void OnPaddingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ListElement listElement)
        {
            listElement.UpdatePadding((Thickness)newValue);
        }
    }

    private void UpdatePadding(Thickness newPadding)
    {
        if (Content is Frame frame)
        {
            frame.Padding = newPadding;
        }
    }
    public ListElement()
    {
        InitializeComponent();
    }
}