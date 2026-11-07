internal class Clipboard
{
	public List<(IEnumerable<FileSystemInfo> Info, ClipboardItemType Type)> Values { get; } = [];

	public void Capture(IEnumerable<FileSystemInfo> items, ClipboardItemType type)
	{
		if (items.Any() == false)
		{
			return;
		}
		Values.Add((items, type));
	}

	public void Clear()
	{
		Values.Clear();
	}

	public enum ClipboardItemType
	{
		Parent,
		Child,
	}
}
