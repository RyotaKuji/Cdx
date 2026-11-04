using Spectre.Console;
using Spectre.Console.Rendering;

internal class Input_V
{
	public string? ReadLine(string prompt)
	{
		Console.SetCursorPosition(0, 0);
		Console.Write("\x1b[2K");
		Console.Write(prompt);
		Console.CursorVisible = true;
		return Console.ReadLine();
	}

	public ConsoleKeyInfo ReadKey(string prompt)
	{
		Console.SetCursorPosition(0, 0);
		Console.Write("\x1b[2K");
		Console.Write(prompt);
		Console.CursorVisible = false;
		return Console.ReadKey(true);
	}

	public void AddBufferText(string text)
	{
		ConsoleInput.Write(text);
	}

	public IRenderable GetComp()
	{
		Text text = new("");
		return text;
	}
}
