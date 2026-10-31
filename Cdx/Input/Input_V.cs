internal class Input_V
{
	const int CursorTop = 1;
	public string ReadLine()
	{
		return Console.ReadLine() ?? string.Empty;
	}

	public ConsoleKeyInfo ReadKey()
	{
		return Console.ReadKey(true);
	}

	public void AddBufferText(string text)
	{
		ConsoleInput.Write(text);
	}

	public void Render()
	{
		Console.SetCursorPosition(0, CursorTop);
		Console.Write("\x1b[2K");
		Console.Write("> ");
	}
}
