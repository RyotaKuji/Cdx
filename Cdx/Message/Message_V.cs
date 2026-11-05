using Spectre.Console;
using Spectre.Console.Rendering;

internal class Message_V(Message model)
{
	public bool HasValue => string.IsNullOrEmpty(model.Value) == false;
	public IRenderable GetComp()
	{
		Text text = new(model.Value);
		return text;
	}
}
