using Spectre.Console;
using Spectre.Console.Rendering;

internal class Renderer
{
	readonly Layout layout = new();

	readonly Layout layout_input = new() { Size = 1 };
	readonly Layout layout_searchTerms = new() { Size = 1 };
	readonly Layout layout_currentDirectory = new() { Size = 1 };
	readonly Layout layout_message = new();
	readonly Layout layout_mainColumns = new();
	readonly Layout layout_mainColumn_left = new() { Ratio = 3 };
	readonly Layout layout_mainColumn_right = new() { Ratio = 1 };

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
		layout.SplitRows(
				layout_input,
				layout_searchTerms,
				layout_currentDirectory,
				layout_message,
				layout_mainColumns);
		layout_mainColumns.SplitColumns(layout_mainColumn_left, layout_mainColumn_right);
	}

	public void Render()
	{
		IRenderable comp_input = input.GetComp();
		IRenderable comp_searchTerms = searchTerms.GetComp();
		IRenderable comp_currentDirectory = currentDirectory.GetComp();
		IRenderable comp_message = message.GetComp();

		layout_input.Update(comp_input);
		layout_searchTerms.Update(comp_searchTerms);
		layout_message.Update(comp_message);
		layout_message.IsVisible = message.HasValue;
		layout_mainColumns.IsVisible = message.HasValue == false;
		layout_currentDirectory.Update(comp_currentDirectory);

		int mainHeight = Console.WindowHeight - (message.HasValue ? 5 : 4);
		int childrenWidth;
		int childrenColumnsCount;

		if (clipboard.HasValue)
		{
			childrenWidth = Console.WindowWidth / 4 * 3;
			childrenColumnsCount = 3;

			IRenderable comp_clipboard = clipboard.GetComp(mainHeight);
			layout_mainColumn_right.Update(comp_clipboard);
		}
		else
		{
			childrenWidth = Console.WindowWidth;
			childrenColumnsCount = 4;
		}

		IRenderable comp_children = children.GetComp(childrenColumnsCount, childrenWidth, mainHeight);
		layout_mainColumn_left.Update(comp_children);
		layout_mainColumn_right.IsVisible = clipboard.HasValue;

		AnsiConsole.Clear();
		AnsiConsole.Write(layout);
	}
}
