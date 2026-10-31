internal class CurrentDirectory_V
{
	const int CursorTop = 0;

	public void Render(string value)
	{
		Console.SetCursorPosition(0, CursorTop);
		Console.Write("\x1b[2K");
		Console.WriteLine(value);
	}
}
