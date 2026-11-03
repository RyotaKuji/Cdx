internal class Children
{
	public IEnumerable<FileSystemInfo> Values { get; private set; } = [];

	public IEnumerable<FileSystemInfo> FilteredValue { get; private set; } = [];

	public void SetDirectory(string path)
	{
		if (Directory.Exists(path))
		{
			Values = new DirectoryInfo(path)
				.GetFileSystemInfos()
				.OrderBy(x => x is FileInfo)
				.ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}
	}

	public void Filter(IEnumerable<string> searchTerms)
	{
		FilteredValue = Values.Where(item => searchTerms.All(term => item.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
	}
}
