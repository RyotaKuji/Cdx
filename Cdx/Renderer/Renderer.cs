using Spectre.Console;
using Spectre.Console.Rendering;

internal class Renderer
{
	readonly Layout layout = new();

	readonly Layout layout_input = new() { Size = 1 };
	readonly Layout layout_searchTerm = new() { Size = 1 };
	readonly Layout layout_message = new();
	readonly Layout layout_main = new();
	readonly Layout layout_main_left = new() { Ratio = 3 };
	readonly Layout layout_main_right = new() { Ratio = 1 };

	readonly CurrentDirectory_V currentDirectory;
	readonly Input_V input;
	readonly SearchTerms_V searchTerms;
	readonly Children_V children;
	readonly Clipboard_V clipboard;
	readonly Message_V message;

	public Renderer(
		Input_V input,
		SearchTerms_V searchTerms,
		CurrentDirectory_V currentDirectory,
		Children_V children,
		Clipboard_V clipboard,
		Message_V message)
	{
		this.input = input;
		this.searchTerms = searchTerms;
		this.currentDirectory = currentDirectory;
		this.children = children;
		this.clipboard = clipboard;
		this.message = message;
		Layout layout_blank = new Layout().Update(new Rule()).Size(1);
		layout.SplitRows(
				layout_input,
				layout_searchTerm,
				layout_message,
				layout_blank,
				layout_main);
		layout_main.SplitColumns(layout_main_left, layout_main_right);
		layout_blank.Update(new Rule() { Border = BoxBorder.None });
	}

	public void Render()
	{
		Console.Clear();
		IRenderable comp_input = input.GetComp();
		IRenderable comp_searchTerms = searchTerms.GetComp();
		layout_input.Update(comp_input);
		layout_searchTerm.Update(comp_searchTerms);

		if (message.HasValue)
		{
			IRenderable comp_message = message.GetComp();
			layout_message.Update(comp_message);
			layout_message.Size = null;
		}
		else
		{
			IRenderable comp_currentDirectory = currentDirectory.GetComp();
			layout_message.Update(comp_currentDirectory);
			layout_message.Size = 1;
		}

		int childrenWidth;
		int childrenColumnsCount;

		if (clipboard.HasValue)
		{
			childrenWidth = Console.WindowWidth / 4 * 3;
			childrenColumnsCount = 3;

			IRenderable comp_clipboard = clipboard.GetComp();
			layout_main_right.Update(comp_clipboard);
		}
		else
		{
			childrenWidth = Console.WindowWidth;
			childrenColumnsCount = 4;
		}

		IRenderable comp_children = children.GetComp(childrenColumnsCount, childrenWidth, 10, 0);
		layout_main_left.Update(comp_children);
		layout_main_right.IsVisible = clipboard.HasValue;

		AnsiConsole.Write(layout);
	}
}
