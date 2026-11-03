using Spectre.Console;
using Spectre.Console.Rendering;

internal class Message_V(Message model)
{
	public bool HasValue => string.IsNullOrEmpty(model.Value) == false;
	public IRenderable GetComp()
	{
		Panel panel = new(model.Value);
		return panel;
	}
}
