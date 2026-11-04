internal class Clipboard
{
	public List<IEnumerable<FileSystemInfo>> Values { get; } = [];

	public void Capture(IEnumerable<FileSystemInfo> items)
	{
		Values.Add(items);
	}

	public void Clear()
	{
		Values.Clear();
	}
}
