internal class FileCommand_V
{
	public void Render(string directory, IEnumerable<string> items)
	{
		Console.SetCursorPosition(0, Console.CursorTop);
		Console.Write("\x1b[2K");
		Console.WriteLine($"Directory: {directory}");
		foreach (var item in items)
		{
			Console.WriteLine(item);
		}
	}
}
