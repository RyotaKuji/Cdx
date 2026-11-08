using Spectre.Console;
using Spectre.Console.Rendering;

internal class Input_V
{
	public string? ReadLine(Input.InputMode mode)
	{
		Console.SetCursorPosition(0, 0);
		Console.Write("\x1b[2K");
		ConsoleColor originalColor = Console.ForegroundColor;
		Console.ForegroundColor = inputMode_color[mode];
		Console.Write(inputMode_prompt[mode]);
		Console.ForegroundColor = originalColor;
		Console.CursorVisible = true;
		return Console.ReadLine();
	}

	public ConsoleKeyInfo ReadKey(Input.InputMode mode)
	{
		Console.SetCursorPosition(0, 0);
		Console.Write("\x1b[2K");
		ConsoleColor originalColor = Console.ForegroundColor;
		Console.ForegroundColor = inputMode_color[mode];
		Console.Write(inputMode_prompt[mode]);
		Console.ForegroundColor = originalColor;
		Console.CursorVisible = false;
		return Console.ReadKey(true);
	}

	public void AddBufferText(string text)
	{
		ConsoleInput.Write(text);
	}

	public IRenderable GetComp()
	{
		Text text = Text.Empty;
		return text;
	}

	public readonly Dictionary<Input.InputMode, string> inputMode_prompt = new()
	{
		[Input.InputMode.Input] = "> ",
		[Input.InputMode.AppCommand] = "[CMD]",
		[Input.InputMode.RowCommand] = "[ROW] ",
	};
	public readonly Dictionary<Input.InputMode, ConsoleColor> inputMode_color = new()
	{
		[Input.InputMode.Input] = ConsoleColor.White,
		[Input.InputMode.AppCommand] = ConsoleColor.Cyan,
		[Input.InputMode.RowCommand] = ConsoleColor.Yellow,
	};
}
