internal class Children_V
{
	const int CursorTop = 3;
	public void Render(IEnumerable<FileSystemInfo> values)
	{
		Console.SetCursorPosition(0, CursorTop);
		Console.Write("\x1b[J");

		foreach (FileSystemInfo info in values)
		{
			string marker = info is DirectoryInfo ? "📁" : "📄";
			Console.WriteLine($"{marker} {info.Name}");
		}
	}
}
