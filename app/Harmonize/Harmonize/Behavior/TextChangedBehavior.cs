using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace Harmonize.Behavior;

public class TextChangedBehavior : Behavior<Editor>
{
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command),
        typeof(ICommand),
        typeof(TextChangedBehavior),
        null);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttachedTo(Editor editor)
    {
        base.OnAttachedTo(editor);
        editor.TextChanged += OnTextChanged;
        editor.BindingContextChanged += OnBindingContextChanged;
        BindingContext = editor.BindingContext; // Ensure correct BindingContext
    }

    protected override void OnDetachingFrom(Editor editor)
    {
        base.OnDetachingFrom(editor);
        editor.TextChanged -= OnTextChanged;
        editor.BindingContextChanged -= OnBindingContextChanged;
    }

    private void OnBindingContextChanged(object sender, EventArgs e)
    {
        BindingContext = ((BindableObject)sender).BindingContext;
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (Command != null && Command.CanExecute(e.NewTextValue))
        {
            Command.Execute(e.NewTextValue);
        }
    }
}

