using Spectre.Console;
using Spectre.Console.Rendering;

internal class CurrentDirectory_V(CurrentDirectory model)
{
	public IRenderable GetComp()
	{
		TextPath textPath = new(model.Value)
		{
			RootStyle = new(Color.Gray),
			StemStyle = new(Color.Gray),
			LeafStyle = new(Color.LightGreen),
			SeparatorStyle = new(Color.Gray),
		};
		return textPath;
	}
}
