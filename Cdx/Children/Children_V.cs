internal class Children_V
{
	const int CursorTop = 3;
	public void Render(IEnumerable<FileSystemInfo> values)
	{
		Console.SetCursorPosition(0, CursorTop);
		Console.Write("\x1b[J");

		if (values.Any() == false)
		{
			Console.WriteLine("(no items match)");
			return;
		}

		int startTop = Console.CursorTop;
		int startLeft = 0;
		int currentTop = startTop;
		int currentLeft = startLeft;
		int maxRow = Console.WindowHeight - 1;

		int maxItemWidth = values
			.Select(item => GetItemText(item).Length)
			.DefaultIfEmpty(0)
			.Max();

		int columnWidth = Math.Clamp(maxItemWidth, 1, Console.WindowWidth / 3);

		foreach (FileSystemInfo item in values)
		{
			if (currentTop > maxRow)
			{
				currentTop = startTop;
				currentLeft += columnWidth;
			}

			if (currentLeft >= Console.WindowWidth)
			{
				break;
			}

			string text = GetItemText(item);

			Console.SetCursorPosition(currentLeft, currentTop);
			int writableWidth = Math.Max(1, Console.WindowWidth - currentLeft);
			if (text.Length > writableWidth)
			{
				text = text[..Math.Max(0, writableWidth - 1)];
			}
			Console.Write(text);

			currentTop++;
		}
	}

	static string GetItemText(FileSystemInfo item)
	{
		string marker = item is DirectoryInfo ? "📁" : "📄";
		return $"{marker} {item.Name}";
	}
}
