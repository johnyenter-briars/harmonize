using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		get
		{
			return (ICommand)GetValue(CommandProperty);
		}
		set
		{
			SetValue(CommandProperty, value);
		}
	}

	protected override void OnAttachedTo(Editor editor)
	{
		editor.TextChanged += OnTextChanged;
		base.OnAttachedTo(editor);
	}

	protected override void OnDetachingFrom(Editor editor)
	{
		editor.TextChanged -= OnTextChanged;
		base.OnDetachingFrom(editor);
	}

	private void OnTextChanged(object sender, TextChangedEventArgs e)
	{
		// I know this is horrible - and breaking the pattern of MVVM, but it's currently 2AM and I wish to return to the sweet bosom of morbius
		if (!string.IsNullOrEmpty(e.NewTextValue))
		{
			//var kodiClient = DependencyService.Get<KodiClient>();

			//Task.Run(async () => await kodiClient.InputText(e.NewTextValue));
		}
		/*
        The above code SHOULD be:
            if (Command != null && Command.CanExecute(e.NewTextValue))
            {
                Command.Execute(e.NewTextValue);
            }
        And should be bound in the content page like so:
            <Editor.Behaviors>
                <Behaviors:TextChangedBehavior Command="{Binding EnterTextChangedCommand}"/>
            </Editor.Behaviors>
        */
	}
}
