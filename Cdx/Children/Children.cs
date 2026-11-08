internal class Children
{
	public IEnumerable<FileSystemInfo> Values { get; private set; } = [];

	public IEnumerable<FileSystemInfo> FilteredValue { get; private set; } = [];

	public void SetDirectory(DirectoryInfo path)
	{
		if (path.Exists)
		{
			Values = path
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

	public FileSystemInfo? GetBestMatchItem() => FilteredValue.MinBy(x => x.Name.Length);
}
