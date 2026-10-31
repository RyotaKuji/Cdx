internal class Message_V
{
	public void Render(string value)
	{
		Console.SetCursorPosition(0, Console.CursorTop);
		Console.Write("\x1b[2K");
		Console.WriteLine(value);
	}
}
