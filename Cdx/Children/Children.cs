internal class Children(Children_V view)
{
	public IEnumerable<FileSystemInfo> Values => values;
	IEnumerable<FileSystemInfo> values = [];
	public IEnumerable<FileSystemInfo> FilteredValue => filteredValues;
	IEnumerable<FileSystemInfo> filteredValues = [];

	public void SetDirectory(string path)
	{
		if (Directory.Exists(path))
		{
			values = new DirectoryInfo(path)
				.GetFileSystemInfos()
				.OrderBy(x => x is FileInfo)
				.ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}
		else
		{
		}
	}

	public void Filter(IEnumerable<string> searchTerms)
	{
		filteredValues = Values.Where(item => searchTerms.All(term => item.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
	}

	public void Render()
	{
		view.Render(FilteredValue);
	}
}
