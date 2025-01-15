namespace Harmonize.Components;

public partial class BottomMenu : ContentView
{
    public BottomMenu()
    {
        InitializeComponent();
    }

    public async Task ShowAsync()
    {
        IsVisible = true;
        await this.TranslateTo(0, 0, 400, Easing.CubicOut);
    }

    public async Task HideAsync()
    {
        await this.TranslateTo(0, 50, 400, Easing.CubicIn);
        IsVisible = false;
    }
}