internal class Children(Children_V view)
{
	public IEnumerable<FileSystemInfo> Values => values;
	IEnumerable<FileSystemInfo> values = [];
	public IEnumerable<FileSystemInfo> FilteredValue => filteredValues;
	IEnumerable<FileSystemInfo> filteredValues = [];

	string currentDirectory = string.Empty;

	public void SetDirectory(string path)
	{
		if (currentDirectory == path)
		{
			return;
		}
		currentDirectory = path;

		if (Directory.Exists(path))
		{
			values = new DirectoryInfo(path).GetFileSystemInfos();
		}
		else
		{
		}
	}

	public void Filter(IEnumerable<string> searchTerms)
	{
		filteredValues = Values.Where(item => searchTerms.All(term => item.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
		view.Render(filteredValues);
	}
}
