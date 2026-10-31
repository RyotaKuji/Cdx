internal class Input_V
{
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

	public void Render(string prompt)
	{
		Console.SetCursorPosition(0, Console.CursorTop);
		Console.Write("\x1b[2K");
		Console.Write(prompt);
	}
}
