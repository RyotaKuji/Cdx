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

	public void RemoveLast()
	{
		if (Values.Count > 0)
		{
			Values.RemoveAt(Values.Count - 1);
		}
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
