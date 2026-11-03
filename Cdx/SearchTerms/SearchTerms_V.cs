using Spectre.Console;
using Spectre.Console.Rendering;

internal class SearchTerms_V(SearchTerms model)
{
	public IRenderable GetComp()
	{
		Text text = new(string.Join(" | ", model.Values));
		return text;
	}
}
