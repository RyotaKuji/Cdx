using Spectre.Console;
using Spectre.Console.Rendering;

internal class Renderer
{
	readonly Layout layout = new();

	readonly Layout layout_input = new() { Size = 1 };
	readonly Layout layout_searchTerm = new() { Size = 1 };
	readonly Layout layout_currentDirectory = new() { Size = 1 };
	readonly Layout layout_main = new();
	readonly Layout layout_children = new() { Ratio = 3 };
	readonly Layout layout_clipboard = new() { Ratio = 1 };

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
				layout_currentDirectory,
				layout_blank,
				layout_main);
		layout_main.SplitColumns(layout_children, layout_clipboard);
		layout_blank.Update(new Rule() { Border = BoxBorder.None });
	}

	public void Render()
	{
		Console.Clear();
		IRenderable comp_input = input.GetComp();
		IRenderable comp_searchTerms = searchTerms.GetComp();
		IRenderable comp_currentDirectory = currentDirectory.GetComp();
		layout_input.Update(comp_input);
		layout_searchTerm.Update(comp_searchTerms);
		layout_currentDirectory.Update(comp_currentDirectory);

		if (clipboard.HasValue)
		{
			int childrenWidth = Console.WindowWidth / 4 * 3;

			IRenderable comp_children = children.GetComp(3, childrenWidth, 10, 0);
			layout_children.Update(comp_children);

			int clipboardWidth = Console.WindowWidth - childrenWidth;
			IRenderable comp_clipboard = clipboard.GetComp(clipboardWidth);
			layout_clipboard.Update(comp_clipboard);
			layout_clipboard.IsVisible = true;
		}
		else
		{
			int childrenWidth = Console.WindowWidth;

			IRenderable comp_children = children.GetComp(4, childrenWidth, 10, 0);
			layout_children.Update(comp_children);

			layout_clipboard.IsVisible = false;
		}

		AnsiConsole.Write(layout);
	}
}
