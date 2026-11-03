using Spectre.Console;
using Spectre.Console.Rendering;

internal class Renderer
{
	readonly Layout layout = new();

	readonly Layout layout_currentDirectory = new() { Size = 1 };
	readonly Layout layout_input = new() { Size = 1 };
	readonly Layout layout_searchTerm = new() { Size = 1 };
	readonly Layout layout_main = new();
	readonly Layout layout_children = new();
	readonly Layout layout_clip = new();

	readonly CurrentDirectory_V currentDirectory;
	readonly Input_V input;
	readonly SearchTerms_V searchTerms;
	readonly Children_V children;
	readonly Message_V message;

	public Renderer(
		CurrentDirectory_V currentDirectory,
		Input_V input,
		SearchTerms_V searchTerms,
		Children_V children,
		Message_V message)
	{
		this.currentDirectory = currentDirectory;
		this.input = input;
		this.searchTerms = searchTerms;
		this.children = children;
		this.message = message;
		Layout layout_blank = new Layout().Update(new Rule()).Size(1);
		layout.SplitRows(
				layout_input,
				layout_searchTerm,
				layout_blank,
				layout_currentDirectory,
				layout_blank,
				layout_main);
		layout_main.SplitColumns(layout_children, layout_clip);
		layout_blank.Update(new Rule() { Border = BoxBorder.None });
	}

	public void Render()
	{
		Console.Clear();
		IRenderable comp_input = input.GetComp();
		IRenderable comp_searchTerms = searchTerms.GetComp();
		IRenderable comp_currentDirectory = currentDirectory.GetComp();
		IRenderable comp_children = children.GetComp(3, Console.WindowWidth, 10, 0);
		layout_input.Update(comp_input);
		layout_searchTerm.Update(comp_searchTerms);
		layout_currentDirectory.Update(comp_currentDirectory);
		layout_children.Update(comp_children);
		layout_clip.IsVisible = false;

		AnsiConsole.Write(layout);
	}
}
