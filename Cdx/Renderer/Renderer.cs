using Spectre.Console;
using Spectre.Console.Rendering;

internal class Renderer
{
	readonly Layout layout = new();

	readonly Layout layout_input = new() { Size = 1 };
	readonly Layout layout_main = new();
	readonly Layout layout_bottomColumns = new();
	readonly Layout layout_bottomColumns_left = new() { Ratio = 3 };
	readonly Layout layout_bottomColumns_right = new() { Ratio = 1 };

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
				layout_main);
		layout_bottomColumns.SplitColumns(layout_bottomColumns_left, layout_bottomColumns_right);
		layout_blank.Update(new Rule() { Border = BoxBorder.None });
	}

	public void Render()
	{
		IRenderable comp_input = input.GetComp();
		IRenderable comp_searchTerms = searchTerms.GetComp();
		IRenderable comp_currentDirectory = currentDirectory.GetComp();
		IRenderable comp_message = message.GetComp();

		layout_input.Update(comp_input);

		int bottomHeight = Console.WindowHeight - 4;
		int childrenWidth;
		int childrenColumnsCount;

		if (clipboard.HasValue)
		{
			childrenWidth = Console.WindowWidth / 4 * 3;
			childrenColumnsCount = 3;

			IRenderable comp_clipboard = clipboard.GetComp(bottomHeight);
			layout_bottomColumns_right.Update(comp_clipboard);
		}
		else
		{
			childrenWidth = Console.WindowWidth;
			childrenColumnsCount = 4;
		}

		IRenderable comp_children = children.GetComp(childrenColumnsCount, childrenWidth, bottomHeight);
		layout_bottomColumns_left.Update(comp_children);
		layout_bottomColumns_right.IsVisible = clipboard.HasValue;

		List<IRenderable> components = [];

		components.Add(comp_searchTerms);
		if (message.HasValue)
		{
			components.Add(comp_message);
		}
		components.Add(comp_currentDirectory);
		components.Add(layout_bottomColumns);

		Rows rows_main = new(components);

		layout_main.Update(rows_main);

		AnsiConsole.Clear();
		AnsiConsole.Write(layout);
	}
}
