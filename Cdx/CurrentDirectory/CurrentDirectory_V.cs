using Spectre.Console;
using Spectre.Console.Rendering;

internal class CurrentDirectory_V(CurrentDirectory model)
{
	public IRenderable GetComp()
	{
		TextPath textPath = new(model.Value);
		return textPath;
	}
}
