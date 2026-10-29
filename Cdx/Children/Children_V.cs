internal class Children_V
{
	public void Render(IEnumerable<FileSystemInfo> values)
	{
		foreach (FileSystemInfo info in values)
		{
			string marker = info is DirectoryInfo ? "📁" : "📄";
			Console.WriteLine($"{marker} {info.Name}");
		}
	}
}
