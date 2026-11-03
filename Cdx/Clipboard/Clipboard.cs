internal class Clipboard
{
	public OrderedDictionary<string, IEnumerable<FileSystemInfo>> Values { get; } = [];

	public void Capture(string directory, IEnumerable<FileSystemInfo> items)
	{
		if (Values.TryGetValue(directory, out var existingItems))
		{
			Values[directory] = existingItems.Concat(items).DistinctBy(info => info.FullName);
		}
		else
		{
			Values[directory] = items;
		}
	}

	public void Copy(string directory)
	{

	}

	public void Move(string directory)
	{
	}

	public void Clear()
	{
		Values.Clear();
	}
}
