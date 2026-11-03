internal class FileCommand(FileCommand_V view)
{
	public OrderedDictionary<string, IEnumerable<string>> Parent_Items { get; } = [];

	public void Capture(string directory, IEnumerable<string> items)
	{
		if (Parent_Items.TryGetValue(directory, out var existingItems))
		{
			Parent_Items[directory] = existingItems.Concat(items);
		}
		else
		{
			Parent_Items[directory] = items;
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
		Parent_Items.Clear();
	}

	public void Render(string directory)
	{
	}
}
