internal class Clipboard
{
	public List<IEnumerable<FileSystemInfo>> Values { get; } = [];

	public void Capture(IEnumerable<FileSystemInfo> items)
	{
		if (items.Any() == false)
		{
			return;
		}
		Values.Add(items);
	}

	public void Clear()
	{
		Values.Clear();
	}
}
